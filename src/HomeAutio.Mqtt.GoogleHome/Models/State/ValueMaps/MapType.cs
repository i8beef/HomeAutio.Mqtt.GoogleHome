using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    public enum MapType
    {
        [EnumMember(Value = "value")]
        Value,
        [EnumMember(Value = "range")]
        Range,
        [EnumMember(Value = "static")]
        Static
    }
}
