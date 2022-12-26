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
        public required string RequestId { get; init; }

        /// <summary>
        /// Inputs.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(IntentJsonConverter))]
        public required IList<IntentBase> Inputs { get; init; }
    }
}
