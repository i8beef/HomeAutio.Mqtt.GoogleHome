using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public enum IntentType
    {
        [EnumMember(Value = "action.devices.SYNC")]
        Sync,
        [EnumMember(Value = "action.devices.QUERY")]
        Query,
        [EnumMember(Value = "action.devices.EXECUTE")]
        Execute,
        [EnumMember(Value = "action.devices.DISCONNECT")]
        Disconnect
    }
}
