using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public enum GoogleType
    {
        [EnumMember(Value = "string")]
        String,
        [EnumMember(Value = "numeric")]
        Numeric,
        [EnumMember(Value = "bool")]
        Bool
    }
}
