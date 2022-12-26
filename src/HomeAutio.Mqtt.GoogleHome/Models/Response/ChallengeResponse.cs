namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Challenge response.
    /// </summary>
    public class ChallengeResponse
    {
        /// <summary>
        /// Type of challenge needed.
        /// </summary>
        public required string Type { get; init; }
    }
}
