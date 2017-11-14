using DigitalTwinApp.DeviceDigiTwin.Interfaces;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwinApp.TelemetryReceiver
{
    public class TelemetryEventProcessor : IEventProcessor
    {
        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            //Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            //Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            //return Task.CompletedTask;
            return Task.FromResult<object>(null);
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            //Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            //return Task.CompletedTask;
            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                try
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    var jsonArrayMessage = JArray.Parse(data);
                    foreach(var obj in jsonArrayMessage.Children<JObject>())
                    {

                        string deviceId = string.Empty;
                        deviceId = obj.Property("deviceId").Value.Value<string>();
                        Guid deviceIdGuid = Guid.Parse(deviceId);

                        int temperature = obj.Property("temperature").Value.Value<int>();

                        IDeviceDigiTwin deviceActor = ActorProxy.Create<IDeviceDigiTwin>(new ActorId(Convert.ToString(deviceId)), new Uri("fabric:/DigitalTwinApp/DeviceDigiTwinActorService"));
                        deviceActor.SetTemperatureAsync(temperature);
                    }

                    //eventData.Properties
                    Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
                }
                catch (Exception ex) { }
            }

            return context.CheckpointAsync();
        }

    }
}
