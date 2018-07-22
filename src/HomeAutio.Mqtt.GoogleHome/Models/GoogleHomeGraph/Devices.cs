using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph
{
    /// <summary>
    /// Devices request object.
    /// </summary>
    public class Devices
    {
        /// <summary>
        /// States.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(ObjectDictionaryConverter))]
        public IDictionary<string, IDictionary<string, object>> States { get; set; }
    }
}
