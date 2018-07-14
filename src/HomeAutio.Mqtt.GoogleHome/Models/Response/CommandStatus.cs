using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    public enum CommandStatus
    {
        [EnumMember(Value = "PENDING")]
        Pending,
        [EnumMember(Value = "SUCCESS")]
        Success,
        [EnumMember(Value = "OFFLINE")]
        Offline,
        [EnumMember(Value = "ERROR")]
        Error
    }
}
