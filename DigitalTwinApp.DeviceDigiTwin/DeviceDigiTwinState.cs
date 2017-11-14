using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwinApp.DeviceDigiTwin
{
    [DataContract]
    public class DeviceDigiTwinState
    {
        public DeviceDigiTwinState()
        {
            DeviceInstance = new ConcurrentDictionary<string, string>();
        }

        [DataMember]
        public Guid DeviceSerialNumber { get; set; }

        [DataMember]
        public ConcurrentDictionary<string,string> DeviceInstance { get; set; }
    }
}
