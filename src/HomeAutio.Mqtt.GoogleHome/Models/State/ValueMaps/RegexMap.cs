using System.Text.RegularExpressions;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Regex based mapper.
    /// </summary>
    public class RegexMap : MapBase
    {
        /// <summary>
        /// Regex to use when converting from MQTT to Google.
        /// </summary>
        public required string GoogleSearch { get; init; }

        /// <summary>
        /// Replacement string to use when converting from MQTT to Google.
        /// </summary>
        public required string GoogleReplace { get; init; }

        /// <summary>
        /// Regex to use when converting from Google to MQTT.
        /// </summary>
        public required string MqttSearch { get; init; }

        /// <summary>
        /// Replacement string to use when converting from Google to MQTT.
        /// </summary>
        public required string MqttReplace { get; init; }

        /// <inheritdoc />
        public override bool MatchesGoogle(object? value)
        {
            if (!string.IsNullOrWhiteSpace(GoogleSearch) && value is not null)
            {
                var stringValue = value.ToString();
                return Regex.IsMatch(stringValue!, GoogleSearch);
            }

            return false;
        }

        /// <inheritdoc />
        public override string? ConvertToGoogle(string? value)
        {
            if (!string.IsNullOrWhiteSpace(MqttSearch)
                && value is not null)
            {
                return Regex.Replace(value, MqttSearch, GoogleReplace);
            }

            return value;
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string? value)
        {
            if (!string.IsNullOrWhiteSpace(MqttSearch) && value is not null)
            {
                return Regex.IsMatch(value, MqttSearch);
            }

            return false;
        }

        /// <inheritdoc />
        public override string? ConvertToMqtt(object? value)
        {
            if (!string.IsNullOrWhiteSpace(GoogleSearch)
                && value is not null)
            {
                var stringValue = value.ToString();
                return Regex.Replace(stringValue!, GoogleSearch, MqttReplace);
            }

            return value?.ToString();
        }
    }
}
