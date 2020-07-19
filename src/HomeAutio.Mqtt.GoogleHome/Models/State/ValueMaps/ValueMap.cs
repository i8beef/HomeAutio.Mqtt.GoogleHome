using System.Collections.Generic;
using System.Linq;

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
        public string Google { get; set; }

        /// <summary>
        /// MQTT value.
        /// </summary>
        public string Mqtt { get; set; }

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
            return value == Mqtt;
        }

        /// <inheritdoc />
        public override string ConvertToMqtt(object value)
        {
            return Mqtt;
        }
    }
}
