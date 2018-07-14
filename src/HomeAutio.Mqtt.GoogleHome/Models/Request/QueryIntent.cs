namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class QueryIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Query;
        public QueryIntentPayload Payload { get; set; }
    }
}
