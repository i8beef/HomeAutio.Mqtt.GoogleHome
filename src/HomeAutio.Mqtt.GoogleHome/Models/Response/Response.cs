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
        public string RequestId { get; set; }

        /// <summary>
        /// Payload.
        /// </summary>
        public object Payload { get; set; }
    }
}
