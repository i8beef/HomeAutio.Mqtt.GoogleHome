namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class DisconnectIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Disconnect;
    }
}
