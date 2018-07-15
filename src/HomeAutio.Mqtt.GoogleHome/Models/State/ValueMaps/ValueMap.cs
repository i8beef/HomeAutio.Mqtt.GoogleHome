namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Value based value map.
    /// </summary>
    public class ValueMap : MapBase
    {
        /// <summary>
        /// MQTT value.
        /// </summary>
        public string Mqtt { get; set; }

        /// <inheritdoc />
        public override bool MatchesMqtt(string value)
        {
            return value.Equals(Mqtt);
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            return Mqtt;
        }
    }
}
