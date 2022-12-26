using System;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Linear range based value mapper.
    /// </summary>
    public class LinearRangeMap : MapBase
    {
        /// <summary>
        /// Google min value.
        /// </summary>
        public required decimal GoogleMin { get; init; }

        /// <summary>
        /// Google max value.
        /// </summary>
        public required decimal GoogleMax { get; init; }

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

            if (decimal.TryParse(value.ToString(), out var decimalValue))
            {
                return decimalValue >= GoogleMin && decimalValue <= GoogleMax;
            }

            return false;
        }

        /// <inheritdoc />
        public override string? ConvertToGoogle(string? value)
        {
            if (value is null)
            {
                return value;
            }

            if (MqttMax - MqttMin == 0)
            {
                return value;
            }

            if (int.TryParse(value.ToString(), out var intValue))
            {
                // Integer mode
                var newValue = ((intValue - MqttMin) / (MqttMax - MqttMin) * (GoogleMax - GoogleMin)) + GoogleMin;
                return Math.Round(newValue, MidpointRounding.AwayFromZero).ToString();
            }
            else if (decimal.TryParse(value.ToString(), out var decimalValue))
            {
                // Decimal mode
                var newValue = ((decimalValue - MqttMin) / (MqttMax - MqttMin) * (GoogleMax - GoogleMin)) + GoogleMin;
                return newValue.ToString();
            }

            return value;
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

            if (GoogleMax - GoogleMin == 0)
            {
                return value.ToString();
            }

            if (int.TryParse(value.ToString(), out var intValue))
            {
                // Integer mode
                var newValue = ((intValue - GoogleMin) / (GoogleMax - GoogleMin) * (MqttMax - MqttMin)) + MqttMin;
                return Math.Round(newValue, MidpointRounding.AwayFromZero).ToString();
            }
            else if (decimal.TryParse(value.ToString(), out var decimalValue))
            {
                // Decimal mode
                var newValue = ((decimalValue - GoogleMin) / (GoogleMax - GoogleMin) * (MqttMax - MqttMin)) + MqttMin;
                return newValue.ToString();
            }

            return value.ToString();
        }
    }
}
