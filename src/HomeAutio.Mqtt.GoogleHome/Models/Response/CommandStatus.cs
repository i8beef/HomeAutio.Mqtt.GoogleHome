using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Command status enumeration.
    /// </summary>
    public enum CommandStatus
    {
        /// <summary>
        /// PENDING.
        /// </summary>
        [EnumMember(Value = "PENDING")]
        Pending,

        /// <summary>
        /// SUCCESS.
        /// </summary>
        [EnumMember(Value = "SUCCESS")]
        Success,

        /// <summary>
        /// OFFLINE.
        /// </summary>
        [EnumMember(Value = "OFFLINE")]
        Offline,

        /// <summary>
        /// ERROR.
        /// </summary>
        [EnumMember(Value = "ERROR")]
        Error
    }
}
