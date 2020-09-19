using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Command response object.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public Command()
        {
            Ids = new List<string>();
        }

        /// <summary>
        /// Ids executed against.
        /// </summary>
        public IList<string> Ids { get; set; }

        /// <summary>
        /// Status of command.
        /// </summary>
        public CommandStatus Status { get; set; }

        /// <summary>
        /// New states of devices.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> States { get; set; }

        /// <summary>
        /// Debug string.
        /// </summary>
        public string DebugString { get; set; }

        /// <summary>
        /// Error code.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Challenges needed to complete original command.
        /// </summary>
        public ChallengeResponse ChallengeNeeded { get; set; }
    }
}
