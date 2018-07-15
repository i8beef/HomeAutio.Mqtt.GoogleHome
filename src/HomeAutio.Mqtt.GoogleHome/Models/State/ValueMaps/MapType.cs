using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    /// <summary>
    /// Map type enumeration.
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// Value map.
        /// </summary>
        [EnumMember(Value = "value")]
        Value,

        /// <summary>
        /// Range map.
        /// </summary>
        [EnumMember(Value = "range")]
        Range,

        /// <summary>
        /// Static map.
        /// </summary>
        [EnumMember(Value = "static")]
        Static
    }
}
