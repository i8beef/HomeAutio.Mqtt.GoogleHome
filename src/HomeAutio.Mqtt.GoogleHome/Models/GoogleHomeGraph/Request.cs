using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph
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
        /// Agent user id.
        /// </summary>
        [JsonProperty(PropertyName = "agent_user_id")]
        public string AgentUserId { get; set; }

        /// <summary>
        /// Payload.
        /// </summary>
        public QueryResponsePayload Payload { get; set; }
    }
}
