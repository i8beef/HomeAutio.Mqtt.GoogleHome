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
        public bool Ack { get; set; }

        /// <summary>
        /// Pin.
        /// </summary>
        public string Pin { get; set; }
    }
}
