namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class ExecuteIntent : IntentBase
    {
        public override IntentType Intent => IntentType.Execute;
        public ExecuteIntentPayload Payload { get; set; }
    }
}
