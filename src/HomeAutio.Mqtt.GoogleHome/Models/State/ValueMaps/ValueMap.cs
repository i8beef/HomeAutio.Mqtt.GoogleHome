namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    public class ValueMap : MapBase
    {
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
