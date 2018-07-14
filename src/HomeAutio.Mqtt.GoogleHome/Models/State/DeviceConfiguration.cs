using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public class DeviceConfiguration : ReadOnlyDictionary<string, Device>
    {
        public DeviceConfiguration(IDictionary<string, Device> dictionary)
            : base(dictionary)
        {
        }
    }
}
