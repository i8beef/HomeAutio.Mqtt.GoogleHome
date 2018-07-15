using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Request envelope.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Request id.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Inputs.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(IntentJsonConverter))]
        public IList<IntentBase> Inputs { get; set; }
    }
}
