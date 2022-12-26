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
        public required string RequestId { get; init; }

        /// <summary>
        /// Unique identifier per event (for example, a doorbell press).
        /// </summary>
        public required string EventId { get; init; }

        /// <summary>
        /// Agent user id.
        /// </summary>
        public required string AgentUserId { get; init; }

        /// <summary>
        /// Token to maintain state in the follow up notification response.
        /// </summary>
        public string? FollowUpToken { get; init; }

        /// <summary>
        /// Payload.
        /// </summary>
        public required StateAndNotificationPayload Payload { get; init; }
    }
}
