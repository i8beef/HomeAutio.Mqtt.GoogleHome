using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Query response payload response object.
    /// </summary>
    public class QueryResponsePayload
    {
        /// <summary>
        /// Devices.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(ObjectDictionaryConverter))]
        public IDictionary<string, IDictionary<string, object>> Devices { get; set; }
    }
}
