namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Intent base request object.
    /// </summary>
    public abstract class IntentBase
    {
        /// <summary>
        /// Intent type.
        /// </summary>
        public abstract IntentType Intent { get; }
    }
}
