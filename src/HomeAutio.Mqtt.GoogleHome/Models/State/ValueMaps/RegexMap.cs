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
        public string GoogleSearch { get; set; }

        /// <summary>
        /// Replacement string to use when converting from MQTT to Google.
        /// </summary>
        public string GoogleReplace { get; set; }

        /// <summary>
        /// Regex to use when converting from Google to MQTT.
        /// </summary>
        public string MqttSearch { get; set; }

        /// <summary>
        /// Replacement string to use when converting from Google to MQTT.
        /// </summary>
        public string MqttReplace { get; set; }

        /// <inheritdoc />
        public override bool MatchesGoogle(object value)
        {
            if (GoogleSearch == null)
                return false;

            return Regex.IsMatch(value.ToString(), GoogleSearch);
        }

        /// <inheritdoc />
        public override string ConvertToGoogle(string value)
        {
            if (MqttSearch == null || GoogleReplace == null)
                return value;

            return Regex.Replace(value, MqttSearch, GoogleReplace);
        }

        /// <inheritdoc />
        public override bool MatchesMqtt(string value)
        {
            if (MqttSearch == null)
                return false;

            return Regex.IsMatch(value, MqttSearch);
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            if (value == null)
                return null;

            if (GoogleSearch == null || MqttReplace == null)
                return value.ToString();

            return Regex.Replace(value.ToString(), GoogleSearch, MqttReplace);
        }
    }
}
