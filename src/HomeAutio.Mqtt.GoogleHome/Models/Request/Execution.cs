using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class Execution
    {
        public string Command { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Params { get; set; }
    }
}
