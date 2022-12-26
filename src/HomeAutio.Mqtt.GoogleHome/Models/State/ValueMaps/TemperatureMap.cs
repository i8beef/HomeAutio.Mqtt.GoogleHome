namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Celsius to fahrenheit value mapper.
    /// </summary>
    public class TemperatureMap : MapBase
    {
        /// <inheritdoc />
        public override bool MatchesGoogle(object? value)
        {
            return true;
        }

        /// <inheritdoc />
        public override string? ConvertToGoogle(string? value)
        {
            if (decimal.TryParse(value, out var degrees))
            {
                var result = (degrees - 32) * (5 / 9);

                return result.ToString();
            }

            return null;
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string? value)
        {
            return true;
        }

        /// <inheritdoc />
        public override string? ConvertToMqtt(object? value)
        {
            if (value is null)
            {
                return null;
            }

            if (decimal.TryParse(value.ToString(), out var degrees))
            {
                var result = (degrees * (9 / 5)) + 32;

                return result.ToString();
            }

            return null;
        }
    }
}
