namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Execute intent request object.
    /// </summary>
    public class ExecuteIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Execute;

        /// <summary>
        /// Payload.
        /// </summary>
        public required ExecuteIntentPayload Payload { get; init; }
    }
}
