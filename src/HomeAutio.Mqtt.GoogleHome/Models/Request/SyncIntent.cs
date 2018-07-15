namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Sync intent request object.
    /// </summary>
    public class SyncIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Sync;
    }
}
