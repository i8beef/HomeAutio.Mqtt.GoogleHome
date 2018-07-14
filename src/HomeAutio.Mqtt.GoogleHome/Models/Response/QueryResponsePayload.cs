using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    public class QueryResponsePayload
    {
        [JsonProperty(ItemConverterType = typeof(ObjectDictionaryConverter))]
        public IDictionary<string, IDictionary<string, object>> Devices { get; set; }
    }
}
