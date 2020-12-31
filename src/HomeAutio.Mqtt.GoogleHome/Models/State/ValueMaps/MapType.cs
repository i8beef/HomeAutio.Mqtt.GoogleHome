using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Map type enumeration.
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// Celsius map.
        /// </summary>
        [EnumMember(Value = "celsius")]
        Celsius,

        /// <summary>
        /// Range Calc map.
        /// </summary>
        [EnumMember(Value = "linearRange")]
        LinearRange,

        /// <summary>
        /// Range map.
        /// </summary>
        [EnumMember(Value = "range")]
        Range,

        /// <summary>
        /// Regex map.
        /// </summary>
        [EnumMember(Value = "regex")]
        Regex,

        /// <summary>
        /// Static map.
        /// </summary>
        [EnumMember(Value = "static")]
        Static,

        /// <summary>
        /// Value map.
        /// </summary>
        [EnumMember(Value = "value")]
        Value
    }
}
