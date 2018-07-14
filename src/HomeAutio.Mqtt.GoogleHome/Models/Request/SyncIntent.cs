namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class SyncIntent : IntentBase
    {
        public override IntentType Intent => IntentType.Sync;
    }
}
