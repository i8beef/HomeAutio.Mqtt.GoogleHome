using System.Collections.Generic;
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
        public string Command { get; set; }

        /// <summary>
        /// Parameters.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Params { get; set; }
    }
}
