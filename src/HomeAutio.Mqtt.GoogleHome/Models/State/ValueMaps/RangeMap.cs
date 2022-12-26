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
        public required string Google { get; init; }

        /// <summary>
        /// MQTT min value.
        /// </summary>
        public required decimal MqttMin { get; init; }

        /// <summary>
        /// MQTT max value.
        /// </summary>
        public required decimal MqttMax { get; init; }

        /// <inheritdoc />
        public override bool MatchesGoogle(object? value)
        {
            if (value is null)
            {
                return false;
            }

            return value.ToString() == Google;
        }

        /// <inheritdoc />
        public override string? ConvertToGoogle(string? value)
        {
            return Google;
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string? value)
        {
            if (decimal.TryParse(value, out var decimalValue))
            {
                return decimalValue >= MqttMin && decimalValue <= MqttMax;
            }

            return false;
        }

        /// <inheritdoc />
        public override string? ConvertToMqtt(object? value)
        {
            if (value is null)
            {
                return null;
            }

            return value.ToString();
        }
    }
}
