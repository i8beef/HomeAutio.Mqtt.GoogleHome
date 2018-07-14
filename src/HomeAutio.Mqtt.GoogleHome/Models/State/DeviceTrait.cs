using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public class DeviceTrait
    {
        public DeviceTrait()
        {
            Commands = new Dictionary<string, IDictionary<string, string>>();
            State = new Dictionary<string, DeviceState>();
        }

        public string Trait { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Attributes { get; set; }

        public IDictionary<string, IDictionary<string, string>> Commands { get; set; }
        public IDictionary<string, DeviceState> State { get; set; }
    }
}
