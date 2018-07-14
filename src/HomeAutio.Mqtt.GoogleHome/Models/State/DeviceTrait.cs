using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public class DeviceTrait
    {
        public DeviceTrait()
        {
            Commands = new Dictionary<string, IDictionary<string, object>>();
            State = new Dictionary<string, object>();
        }

        public string Trait { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Attributes { get; set; }

        [JsonProperty(ItemConverterType = typeof(ObjectDictionaryConverter))]
        public IDictionary<string, IDictionary<string, object>> Commands { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> State { get; set; }
    }
}
