namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Range based value mapper.
    /// </summary>
    public class RangeMap : MapBase
    {
        /// <summary>
        /// Google value.
        /// </summary>
        public string Google { get; set; }

        /// <summary>
        /// MQTT min value.
        /// </summary>
        public decimal MqttMin { get; set; }

        /// <summary>
        /// MQTT max value.
        /// </summary>
        public decimal MqttMax { get; set; }

        /// <inheritdoc />
        public override bool MatchesGoogle(object value)
        {
            return value.ToString() == Google;
        }

        /// <inheritdoc />
        public override string ConvertToGoogle(string value)
        {
            return Google;
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string value)
        {
            if (decimal.TryParse(value, out var decimalValue))
            {
                return decimalValue >= MqttMin && decimalValue <= MqttMax;
            }

            return false;
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            return value.ToString();
        }
    }
}
