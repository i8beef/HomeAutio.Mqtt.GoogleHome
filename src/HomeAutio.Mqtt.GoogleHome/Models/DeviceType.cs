using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device type enumeration.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// action.devices.types.AC_UNIT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AC_UNIT")]
        AirConditioningUnit,

        /// <summary>
        /// action.devices.types.AIRPURIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRPURIFIER")]
        AirPurifier,

        /// <summary>
        /// action.devices.types.CAMERA.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CAMERA")]
        Camera,

        /// <summary>
        /// action.devices.types.COFFEE_MAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.COFFEE_MAKER")]
        CoffeeMaker,

        /// <summary>
        /// action.devices.types.DISHWASHER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DISHWASHER")]
        Diswasher,

        /// <summary>
        /// action.devices.types.DOOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DOOR")]
        Door,

        /// <summary>
        /// action.devices.types.FAN".
        /// </summary>
        [EnumMember(Value = "action.devices.types.FAN")]
        Fan,

        /// <summary>
        /// action.devices.types.KETTLE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.KETTLE")]
        Kettle,

        /// <summary>
        /// action.devices.types.LIGHT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.LIGHT")]
        Light,

        /// <summary>
        /// action.devices.types.OUTLET.
        /// </summary>
        [EnumMember(Value = "action.devices.types.OUTLET")]
        Outlet,

        /// <summary>
        /// action.devices.types.OVEN.
        /// </summary>
        [EnumMember(Value = "action.devices.types.OVEN")]
        Oven,

        /// <summary>
        /// action.devices.types.REFRIGERATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.REFRIGERATOR")]
        Refrigerator,

        /// <summary>
        /// action.devices.types.SCENE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SCENE")]
        Scene,

        /// <summary>
        /// action.devices.types.SPEAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SPEAKER")]
        Speaker,

        /// <summary>
        /// action.devices.types.SPRINKLER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SPRINKLER")]
        Sprinkler,

        /// <summary>
        /// action.devices.types.SWITCH.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SWITCH")]
        Switch,

        /// <summary>
        /// action.devices.types.THERMOSTAT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.THERMOSTAT")]
        Thermostat,

        /// <summary>
        /// action.devices.types.TV.
        /// </summary>
        [EnumMember(Value = "action.devices.types.TV")]
        TV,

        /// <summary>
        /// action.devices.types.VACUUM.
        /// </summary>
        [EnumMember(Value = "action.devices.types.VACUUM")]
        Vacuum,

        /// <summary>
        /// action.devices.types.WASHER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WASHER")]
        Washer,

        /// <summary>
        /// Unknown device type.
        /// </summary>
        Unknown
    }
}
