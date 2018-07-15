namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Disconnect intent request object.
    /// </summary>
    public class DisconnectIntent : IntentBase
    {
        /// <inheritdoc />
        public override IntentType Intent => IntentType.Disconnect;
    }
}
