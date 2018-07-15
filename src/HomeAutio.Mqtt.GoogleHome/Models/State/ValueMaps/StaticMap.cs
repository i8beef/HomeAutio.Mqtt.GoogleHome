namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Static based value map.
    /// </summary>
    public class StaticMap : MapBase
    {
        /// <inheritdoc />
        public override bool MatchesMqtt(string value)
        {
            return true;
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            return null;
        }
    }
}
