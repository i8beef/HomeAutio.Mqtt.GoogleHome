namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Value map base class.
    /// </summary>
    public abstract class MapBase
    {
        /// <summary>
        /// Map type.
        /// </summary>
        public MapType Type { get; set; }

        /// <summary>
        /// Indicates if map should be applied to the passed MQTT value.
        /// </summary>
        /// <param name="value">MQTT value.</param>
        /// <returns><c>true</c> if it should be applied, else <c>false</c>.</returns>
        public abstract bool MatchesMqtt(string? value);

        /// <summary>
        /// Indicates if map should be applied to the passed Google value.
        /// </summary>
        /// <param name="value">Google value.</param>
        /// <returns><c>true</c> if it should be applied, else <c>false</c>.</returns>
        public abstract bool MatchesGoogle(object? value);

        /// <summary>
        /// Converts the passed value to a valid MQTT value.
        /// </summary>
        /// <param name="value">The Google value to convert.</param>
        /// <returns>The transformed MQTT value.</returns>
        public abstract string? ConvertToMqtt(object? value);

        /// <summary>
        /// Converts the passed value to a valid Google value.
        /// </summary>
        /// <param name="value">The MQTT value to convert.</param>
        /// <returns>The transformed Google value.</returns>
        public abstract string? ConvertToGoogle(string? value);
    }
}
