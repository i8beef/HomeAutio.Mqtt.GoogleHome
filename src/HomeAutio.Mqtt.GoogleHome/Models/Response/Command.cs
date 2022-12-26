using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Command response object.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Ids executed against.
        /// </summary>
        public required IList<string> Ids { get; init; }

        /// <summary>
        /// Status of command.
        /// </summary>
        public required CommandStatus Status { get; init; }

        /// <summary>
        /// New states of devices.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object?>? States { get; init; }

        /// <summary>
        /// Debug string.
        /// </summary>
        public string? DebugString { get; init; }

        /// <summary>
        /// Error code.
        /// </summary>
        public string? ErrorCode { get; init; }

        /// <summary>
        /// Challenges needed to complete original command.
        /// </summary>
        public ChallengeResponse? ChallengeNeeded { get; init; }
    }
}
