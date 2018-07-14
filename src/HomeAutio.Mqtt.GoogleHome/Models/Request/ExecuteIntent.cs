namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class ExecuteIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Execute;
        public ExecuteIntentPayload Payload { get; set; }
    }
}
