namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Query intent request object.
    /// </summary>
    public class QueryIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Query;

        /// <summary>
        /// Payload.
        /// </summary>
        public required QueryIntentPayload Payload { get; init; }
    }
}
