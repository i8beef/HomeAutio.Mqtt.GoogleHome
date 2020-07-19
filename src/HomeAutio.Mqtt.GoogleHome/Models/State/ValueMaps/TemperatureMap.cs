namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Celcius to fahrenheit value mapper.
    /// </summary>
    public class TemperatureMap : MapBase
    {
        /// <inheritdoc />
        public override bool MatchesGoogle(object value)
        {
            return true;
        }

        /// <inheritdoc />
        public override string ConvertToGoogle(string value)
        {
            var degrees = decimal.Parse(value);
            var result = (degrees - 32) * (5 / 9);

            return result.ToString();
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string value)
        {
            return true;
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            var degrees = decimal.Parse(value.ToString());
            var result = (degrees * (9 / 5)) + 32;

            return result.ToString();
        }
    }
}
