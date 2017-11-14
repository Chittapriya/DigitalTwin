using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using DigitalTwinApp.DeviceDigiTwin.Interfaces;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DigitalTwinApp.DeviceDigiTwin
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class DeviceDigiTwin : Actor, IDeviceDigiTwin
    {
        /// <summary>
        /// Initializes a new instance of DeviceDigiTwin
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public DeviceDigiTwin(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            await StateManager.AddOrUpdateStateAsync(typeof(DeviceDigiTwinState).FullName,
                                                        new DeviceDigiTwinState(),
                                                        (key, value) => value);

            await base.OnActivateAsync();
        }

        private async Task<T> GetStateAsync<T>()
        {
            var result = await StateManager.TryGetStateAsync<T>(typeof(T).FullName);
            return result.Value;
        }
        private async Task SetStateAsync<T>(T state)
        {
            await StateManager.SetStateAsync<T>(typeof(T).FullName, state);
        }

        public async Task<int> GetTemperatureAsync()
        {
            var state = await GetStateAsync<DeviceDigiTwinState>();
            string temperatureValue;
            state.DeviceInstance.TryGetValue("temperature", out temperatureValue);
            if (temperatureValue == null)
                throw new Exception("Temperature Property is not available");
            return Int32.Parse(temperatureValue);
        }

        public async Task SetTemperatureAsync(int temperature)
        {
            var state = await GetStateAsync<DeviceDigiTwinState>();
            state.DeviceInstance.AddOrUpdate("temperature",temperature.ToString(), (key,oldvalue) => temperature.ToString());
            await SetStateAsync<DeviceDigiTwinState>(state);
        }

        public async Task UpdateDeviceInstanceAsync(byte[] deviceInstance)
        {
            var instance = JObject.Parse(Encoding.UTF8.GetString(deviceInstance));
            var state = await GetStateAsync<DeviceDigiTwinState>();

            state.DeviceSerialNumber = Guid.Parse(instance.Property("objectSerialNumber").Value.Value<string>());
            state.DeviceInstance.AddOrUpdate(instance.Property("property1").Value.Value<string>(), "-999", (key, oldValue) => "0");
        }
    }
}
