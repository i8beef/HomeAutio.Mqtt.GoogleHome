namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Response envelope.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Request id.
        /// </summary>
        public required string RequestId { get; init; }

        /// <summary>
        /// Payload.
        /// </summary>
        public required object Payload { get; init; }
    }
}
