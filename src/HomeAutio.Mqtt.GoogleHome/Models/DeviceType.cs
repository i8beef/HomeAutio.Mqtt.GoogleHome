using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    public enum DeviceType
    {
        [EnumMember(Value = "action.devices.types.AC_UNIT")]
        AirConditioningUnit,
        [EnumMember(Value = "action.devices.types.AIRPURIFIER")]
        AirPurifier,
        [EnumMember(Value = "action.devices.types.CAMERA")]
        Camera,
        [EnumMember(Value = "action.devices.types.COFFEE_MAKER")]
        CoffeeMaker,
        [EnumMember(Value = "action.devices.types.DISHWASHER")]
        Diswasher,
        [EnumMember(Value = "action.devices.types.FAN")]
        Fan,
        [EnumMember(Value = "action.devices.types.KETTLE")]
        Kettle,
        [EnumMember(Value = "action.devices.types.LIGHT")]
        Light,
        [EnumMember(Value = "action.devices.types.OUTLET")]
        Outlet,
        [EnumMember(Value = "action.devices.types.OVEN")]
        Oven,
        [EnumMember(Value = "action.devices.types.REFRIGERATOR")]
        Refrigerator,
        [EnumMember(Value = "action.devices.types.SCENE")]
        Scene,
        [EnumMember(Value = "action.devices.types.SPRINKLER")]
        Sprinkler,
        [EnumMember(Value = "action.devices.types.SWITCH")]
        Switch,
        [EnumMember(Value = "action.devices.types.THERMOSTAT")]
        Thermostat,
        [EnumMember(Value = "action.devices.types.VACUUM")]
        Vacuum,
        [EnumMember(Value = "action.devices.types.WASHER")]
        Washer
    }
}
