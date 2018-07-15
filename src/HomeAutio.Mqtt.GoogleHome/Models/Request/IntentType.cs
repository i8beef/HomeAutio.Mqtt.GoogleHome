using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Intent type enumeration.
    /// </summary>
    public enum IntentType
    {
        /// <summary>
        /// action.devices.SYNC.
        /// </summary>
        [EnumMember(Value = "action.devices.SYNC")]
        Sync,

        /// <summary>
        /// action.devices.QUERY.
        /// </summary>
        [EnumMember(Value = "action.devices.QUERY")]
        Query,

        /// <summary>
        /// action.devices.EXECUTE.
        /// </summary>
        [EnumMember(Value = "action.devices.EXECUTE")]
        Execute,

        /// <summary>
        /// action.devices.DISCONNECT.
        /// </summary>
        [EnumMember(Value = "action.devices.DISCONNECT")]
        Disconnect
    }
}
