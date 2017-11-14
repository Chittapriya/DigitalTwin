using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
[assembly: FabricTransportActorRemotingProvider(RemotingListener = RemotingListener.V2Listener, RemotingClient = RemotingClient.V2Client)]

namespace DigitalTwinApp.DeviceDigiTwin.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDeviceDigiTwin : IActor
    {

        Task<int> GetTemperatureAsync();

        Task SetTemperatureAsync(int temperature);

        Task UpdateDeviceInstanceAsync(byte[] deviceInstance);
    }
}
