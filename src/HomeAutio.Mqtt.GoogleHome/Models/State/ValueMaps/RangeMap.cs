namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    public class RangeMap : MapBase
    {
        public decimal MqttMin { get; set; }
        public decimal MqttMax { get; set; }

        /// <inheritdoc />
        public override bool MatchesMqtt(string value)
        {
            if (decimal.TryParse(value, out decimal decimalValue))
                return decimalValue >= MqttMin && decimalValue <= MqttMax;

            return false;
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            return value.ToString();
        }
    }
}
