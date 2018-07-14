namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class QueryIntent : IntentBase
    {
        public override IntentType Intent => IntentType.Query;
        public QueryIntentPayload Payload { get; set; }
    }
}
