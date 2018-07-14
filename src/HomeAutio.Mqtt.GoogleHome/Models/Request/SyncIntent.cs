namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class SyncIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Sync;
    }
}
