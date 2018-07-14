namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class DisconnectIntent : IntentBase
    {
        public override IntentType Intent => IntentType.Disconnect;
    }
}
