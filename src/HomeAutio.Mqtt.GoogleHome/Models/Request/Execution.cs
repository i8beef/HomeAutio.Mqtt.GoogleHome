using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Execution request object.
    /// </summary>
    public class Execution
    {
        /// <summary>
        /// Command name.
        /// </summary>
        public required string Command { get; init; }

        /// <summary>
        /// Parameters.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object?>? Params { get; init; }

        /// <summary>
        /// Challenge details.
        /// </summary>
        public Challenge? Challenge { get; init; }
    }
}
