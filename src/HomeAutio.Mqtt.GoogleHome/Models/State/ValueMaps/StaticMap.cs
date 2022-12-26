namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Static based value map.
    /// </summary>
    public class StaticMap : MapBase
    {
        /// <summary>
        /// Google value.
        /// </summary>
        public string? Google { get; init; }

        /// <inheritdoc />
        public override bool MatchesGoogle(object? value)
        {
            return value is null || string.Equals(value.ToString(), Google, System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override string? ConvertToGoogle(string? value)
        {
            return Google;
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string? value)
        {
            return true;
        }

        /// <inheritdoc />
        public override string? ConvertToMqtt(object? value)
        {
            return null;
        }
    }
}
