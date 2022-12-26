namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Value based value map.
    /// </summary>
    public class ValueMap : MapBase
    {
        /// <summary>
        /// Google value.
        /// </summary>
        public string? Google { get; init; }

        /// <summary>
        /// MQTT value.
        /// </summary>
        public string? Mqtt { get; init; }

        /// <inheritdoc />
        public override bool MatchesGoogle(object? value)
        {
            if (value is null)
            {
                return Google is null;
            }

            return string.Equals(value.ToString(), Google, System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override string? ConvertToGoogle(string? value)
        {
            return Google;
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string? value)
        {
            return string.Equals(Mqtt, value, System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override string? ConvertToMqtt(object? value)
        {
            return Mqtt;
        }
    }
}
