using System.Runtime.Serialization;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device type enumeration.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Unknown device type.
        /// </summary>
        Unknown,

        /// <summary>
        /// action.devices.types.AC_UNIT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AC_UNIT")]
        AC_UNIT,

        /// <summary>
        /// action.devices.types.AIRCOOLER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRCOOLER")]
        AIRCOOLER,

        /// <summary>
        /// action.devices.types.AIRFRESHENER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRFRESHENER")]
        AIRFRESHENER,

        /// <summary>
        /// action.devices.types.AIRPURIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRPURIFIER")]
        AIRPURIFIER,

        /// <summary>
        /// action.devices.types.AUDIO_VIDEO_RECEIVER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AUDIO_VIDEO_RECEIVER")]
        AUDIO_VIDEO_RECEIVER,

        /// <summary>
        /// action.devices.types.AWNING.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AWNING")]
        AWNING,

        /// <summary>
        /// action.devices.types.BATHTUB.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BATHTUB")]
        BATHTUB,

        /// <summary>
        /// action.devices.types.BED.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BED")]
        BED,

        /// <summary>
        /// action.devices.types.BLENDER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BLENDER")]
        BLENDER,

        /// <summary>
        /// action.devices.types.BLINDS.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BLINDS")]
        BLINDS,

        /// <summary>
        /// action.devices.types.BOILER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BOILER")]
        BOILER,

        /// <summary>
        /// action.devices.types.CAMERA.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CAMERA")]
        CAMERA,

        /// <summary>
        /// action.devices.types.CARBON_MONOXIDE_DETECTOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CARBON_MONOXIDE_DETECTOR")]
        CARBON_MONOXIDE_DETECTOR,

        /// <summary>
        /// action.devices.types.CHARGER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CHARGER")]
        CHARGER,

        /// <summary>
        /// action.devices.types.CLOSET.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CLOSET")]
        CLOSET,

        /// <summary>
        /// action.devices.types.COFFEE_MAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.COFFEE_MAKER")]
        COFFEE_MAKER,

        /// <summary>
        /// action.devices.types.COOKTOP.
        /// </summary>
        [EnumMember(Value = "action.devices.types.COOKTOP")]
        COOKTOP,

        /// <summary>
        /// action.devices.types.CURTAIN.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CURTAIN")]
        CURTAIN,

        /// <summary>
        /// action.devices.types.DEHUMIDIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DEHUMIDIFIER")]
        DEHUMIDIFIER,

        /// <summary>
        /// action.devices.types.DEHYDRATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DEHYDRATOR")]
        DEHYDRATOR,

        /// <summary>
        /// action.devices.types.DISHWASHER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DISHWASHER")]
        DISHWASHER,

        /// <summary>
        /// action.devices.types.DOOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DOOR")]
        DOOR,

        /// <summary>
        /// action.devices.types.DOORBELL.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DOORBELL")]
        DOORBELL,

        /// <summary>
        /// action.devices.types.DRAWER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DRAWER")]
        DRAWER,

        /// <summary>
        /// action.devices.types.DRYER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DRYER")]
        DRYER,

        /// <summary>
        /// action.devices.types.FAN.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FAN")]
        FAN,

        /// <summary>
        /// action.devices.types.FAUCET.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FAUCET")]
        FAUCET,

        /// <summary>
        /// action.devices.types.FIREPLACE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FIREPLACE")]
        FIREPLACE,

        /// <summary>
        /// action.devices.types.FREEZER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FREEZER")]
        FREEZER,

        /// <summary>
        /// action.devices.types.FRYER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FRYER")]
        FRYER,

        /// <summary>
        /// action.devices.types.GARAGE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.GARAGE")]
        GARAGE,

        /// <summary>
        /// action.devices.types.GATE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.GATE")]
        GATE,

        /// <summary>
        /// action.devices.types.GRILL.
        /// </summary>
        [EnumMember(Value = "action.devices.types.GRILL")]
        GRILL,

        /// <summary>
        /// action.devices.types.HEATER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.HEATER")]
        HEATER,

        /// <summary>
        /// action.devices.types.HOOD.
        /// </summary>
        [EnumMember(Value = "action.devices.types.HOOD")]
        HOOD,

        /// <summary>
        /// action.devices.types.HUMIDIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.HUMIDIFIER")]
        HUMIDIFIER,

        /// <summary>
        /// action.devices.types.KETTLE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.KETTLE")]
        KETTLE,

        /// <summary>
        /// action.devices.types.LIGHT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.LIGHT")]
        LIGHT,

        /// <summary>
        /// action.devices.types.LOCK.
        /// </summary>
        [EnumMember(Value = "action.devices.types.LOCK")]
        LOCK,

        /// <summary>
        /// action.devices.types.MICROWAVE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MICROWAVE")]
        MICROWAVE,

        /// <summary>
        /// action.devices.types.MOP.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MOP")]
        MOP,

        /// <summary>
        /// action.devices.types.MOWER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MOWER")]
        MOWER,

        /// <summary>
        /// action.devices.types.MULTICOOKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MULTICOOKER")]
        MULTICOOKER,

        /// <summary>
        /// action.devices.types.NETWORK.
        /// </summary>
        [EnumMember(Value = "action.devices.types.NETWORK")]
        NETWORK,

        /// <summary>
        /// action.devices.types.OUTLET.
        /// </summary>
        [EnumMember(Value = "action.devices.types.OUTLET")]
        OUTLET,

        /// <summary>
        /// action.devices.types.OVEN.
        /// </summary>
        [EnumMember(Value = "action.devices.types.OVEN")]
        OVEN,

        /// <summary>
        /// action.devices.types.PERGOLA.
        /// </summary>
        [EnumMember(Value = "action.devices.types.PERGOLA")]
        PERGOLA,

        /// <summary>
        /// action.devices.types.PETFEEDER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.PETFEEDER")]
        PETFEEDER,

        /// <summary>
        /// action.devices.types.PRESSURECOOKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.PRESSURECOOKER")]
        PRESSURECOOKER,

        /// <summary>
        /// action.devices.types.RADIATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.RADIATOR")]
        RADIATOR,

        /// <summary>
        /// action.devices.types.REFRIGERATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.REFRIGERATOR")]
        REFRIGERATOR,

        /// <summary>
        /// action.devices.types.REMOTECONTROL.
        /// </summary>
        [EnumMember(Value = "action.devices.types.REMOTECONTROL")]
        REMOTECONTROL,

        /// <summary>
        /// action.devices.types.ROUTER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.ROUTER")]
        ROUTER,

        /// <summary>
        /// action.devices.types.SCENE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SCENE")]
        SCENE,

        /// <summary>
        /// action.devices.types.SECURITYSYSTEM.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SECURITYSYSTEM")]
        SECURITYSYSTEM,

        /// <summary>
        /// action.devices.types.SENSOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SENSOR")]
        SENSOR,

        /// <summary>
        /// action.devices.types.SETTOP.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SETTOP")]
        SETTOP,

        /// <summary>
        /// action.devices.types.SHOWER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SHOWER")]
        SHOWER,

        /// <summary>
        /// action.devices.types.SHUTTER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SHUTTER")]
        SHUTTER,

        /// <summary>
        /// action.devices.types.SMOKE_DETECTOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SMOKE_DETECTOR")]
        SMOKE_DETECTOR,

        /// <summary>
        /// action.devices.types.SOUNDBAR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SOUNDBAR")]
        SOUNDBAR,

        /// <summary>
        /// action.devices.types.SOUSVIDE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SOUSVIDE")]
        SOUSVIDE,

        /// <summary>
        /// action.devices.types.SPEAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SPEAKER")]
        SPEAKER,

        /// <summary>
        /// action.devices.types.SPRINKLER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SPRINKLER")]
        SPRINKLER,

        /// <summary>
        /// action.devices.types.STANDMIXER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STANDMIXER")]
        STANDMIXER,

        /// <summary>
        /// action.devices.types.STREAMING_BOX.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STREAMING_BOX")]
        STREAMING_BOX,

        /// <summary>
        /// action.devices.types.STREAMING_SOUNDBAR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STREAMING_SOUNDBAR")]
        STREAMING_SOUNDBAR,

        /// <summary>
        /// action.devices.types.STREAMING_STICK.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STREAMING_STICK")]
        STREAMING_STICK,

        /// <summary>
        /// action.devices.types.SWITCH.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SWITCH")]
        SWITCH,

        /// <summary>
        /// action.devices.types.THERMOSTAT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.THERMOSTAT")]
        THERMOSTAT,

        /// <summary>
        /// action.devices.types.TV.
        /// </summary>
        [EnumMember(Value = "action.devices.types.TV")]
        TV,

        /// <summary>
        /// action.devices.types.VACUUM.
        /// </summary>
        [EnumMember(Value = "action.devices.types.VACUUM")]
        VACUUM,

        /// <summary>
        /// action.devices.types.VALVE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.VALVE")]
        VALVE,

        /// <summary>
        /// action.devices.types.WASHER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WASHER")]
        WASHER,

        /// <summary>
        /// action.devices.types.WATERHEATER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WATERHEATER")]
        WATERHEATER,

        /// <summary>
        /// action.devices.types.WATERPURIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WATERPURIFIER")]
        WATERPURIFIER,

        /// <summary>
        /// action.devices.types.WATERSOFTENER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WATERSOFTENER")]
        WATERSOFTENER,

        /// <summary>
        /// action.devices.types.WINDOW.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WINDOW")]
        WINDOW,

        /// <summary>
        /// action.devices.types.YOGURTMAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.YOGURTMAKER")]
        YOGURTMAKER
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
