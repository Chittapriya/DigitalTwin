using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DigitalTwinApp.DeviceDigiTwin.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace DigitalTwinApp.TwinMgmtApi.Controllers
{
    [Route("api/[Controller]")]
    public class DeviceDigiTwinController : Controller
    {
        public DeviceDigiTwinController()
        {
           
        }

        [HttpPut("{deviceId}/updatedeviceinstance")]
        public async Task<IActionResult> CreateDeviceInstance(Guid deviceId,[FromBody]JObject deviceInstance)
        {
            IDeviceDigiTwin deviceTwinActor = ActorProxy.Create<IDeviceDigiTwin>(new ActorId(Convert.ToString(deviceId)), new Uri("fabric:/DigitalTwinApp/DeviceDigiTwinActorService"));
            await deviceTwinActor.UpdateDeviceInstanceAsync(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(deviceInstance)));
            return new ObjectResult(deviceId);
        }

        [HttpPut("{deviceId}/updatedevicetemperature")]
        public async Task<IActionResult> UpdateDeviceTemperature(Guid deviceId, [FromBody]JObject temperatureInput)
        {
            var temppurature = Int32.Parse(temperatureInput.Property("temperature").Value.Value<string>());
            IDeviceDigiTwin deviceTwinActor = ActorProxy.Create<IDeviceDigiTwin>(new ActorId(Convert.ToString(deviceId)), new Uri("fabric:/DigitalTwinApp/DeviceDigiTwinActorService"));
            await deviceTwinActor.SetTemperatureAsync(temppurature);
            return new ObjectResult(String.Format("Temperature Updated for {0}",deviceId));
        }

        [HttpGet("{deviceId}/getdevicetemperature")]
        public async Task<IActionResult> GetDeviceTemperature(Guid deviceId)
        {
            IDeviceDigiTwin deviceTwinActor = ActorProxy.Create<IDeviceDigiTwin>(new ActorId(Convert.ToString(deviceId)), new Uri("fabric:/DigitalTwinApp/DeviceDigiTwinActorService"));
            var temperature = await deviceTwinActor.GetTemperatureAsync();
            return new ObjectResult(temperature);
        }


    }
}