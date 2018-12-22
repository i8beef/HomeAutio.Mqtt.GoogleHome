namespace HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph
{
    /// <summary>
    /// Request envelope.
    /// </summary>
    public class ReportStateAndNotificationRequest
    {
        /// <summary>
        /// Request ID used for debugging.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Unique identifier per event (for example, a doorbell press).
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Agent user id.
        /// </summary>
        public string AgentUserId { get; set; }

        /// <summary>
        /// Token to maintain state in the follow up notification response.
        /// </summary>
        public string FollowUpToken { get; set; }

        /// <summary>
        /// Payload.
        /// </summary>
        public StateAndNotificationPayload Payload { get; set; }
    }
}
