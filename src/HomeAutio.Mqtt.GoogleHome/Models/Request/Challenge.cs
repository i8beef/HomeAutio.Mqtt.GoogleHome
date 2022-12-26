namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Challenge.
    /// </summary>
    public class Challenge
    {
        /// <summary>
        /// Acknowledge.
        /// </summary>
        public bool? Ack { get; init; }

        /// <summary>
        /// Pin.
        /// </summary>
        public string? Pin { get; init; }
    }
}
