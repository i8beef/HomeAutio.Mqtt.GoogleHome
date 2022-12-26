using System.Runtime.Serialization;

#pragma warning disable CA1720 // Identifier contains type name
namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    /// <summary>
    /// Google type enumeration.
    /// </summary>
    public enum GoogleType
    {
        /// <summary>
        /// String.
        /// </summary>
        Unknown,

        /// <summary>
        /// String.
        /// </summary>
        [EnumMember(Value = "string")]
        String,

        /// <summary>
        /// Numeric.
        /// </summary>
        [EnumMember(Value = "numeric")]
        Numeric,

        /// <summary>
        /// Boolean.
        /// </summary>
        [EnumMember(Value = "bool")]
        Bool
    }
}
#pragma warning restore CA1720 // Identifier contains type name
