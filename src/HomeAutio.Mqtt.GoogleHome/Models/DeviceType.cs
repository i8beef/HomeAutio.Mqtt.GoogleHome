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
        /// action.devices.types.AIRCOOLER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRCOOLER")]
        AirCooler,

        /// <summary>
        /// action.devices.types.AIRFRESHENER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRFRESHENER")]
        AirFreshener,

        /// <summary>
        /// action.devices.types.AIRPURIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AIRPURIFIER")]
        AirPurifier,

        /// <summary>
        /// action.devices.types.AUDIO_VIDEO_RECEIVER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AUDIO_VIDEO_RECEIVER")]
        AudioVideoReceiver,

        /// <summary>
        /// action.devices.types.AWNING.
        /// </summary>
        [EnumMember(Value = "action.devices.types.AWNING")]
        Awning,

        /// <summary>
        /// action.devices.types.BATHTUB.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BATHTUB")]
        Bathtub,

        /// <summary>
        /// action.devices.types.BED.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BED")]
        Bed,

        /// <summary>
        /// action.devices.types.BLENDER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BLENDER")]
        Blender,

        /// <summary>
        /// action.devices.types.BLINDS.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BLINDS")]
        Blinds,

        /// <summary>
        /// action.devices.types.BOILER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.BOILER")]
        Boiler,

        /// <summary>
        /// action.devices.types.CAMERA.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CAMERA")]
        Camera,

        /// <summary>
        /// action.devices.types.CARBON_MONOXIDE_DETECTOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CARBON_MONOXIDE_DETECTOR")]
        CarbonMonoxideDetector,

        /// <summary>
        /// action.devices.types.CHARGER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CHARGER")]
        Charger,

        /// <summary>
        /// action.devices.types.CLOSET.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CLOSET")]
        Closet,

        /// <summary>
        /// action.devices.types.COFFEE_MAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.COFFEE_MAKER")]
        CoffeeMaker,

        /// <summary>
        /// action.devices.types.COOKTOP.
        /// </summary>
        [EnumMember(Value = "action.devices.types.COOKTOP")]
        Cooktop,

        /// <summary>
        /// action.devices.types.CURTAIN.
        /// </summary>
        [EnumMember(Value = "action.devices.types.CURTAIN")]
        Curtain,

        /// <summary>
        /// action.devices.types.DEHUMIDIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DEHUMIDIFIER")]
        Dehumidifier,

        /// <summary>
        /// action.devices.types.DEHYDRATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DEHYDRATOR")]
        Dehydrator,

        /// <summary>
        /// action.devices.types.DISHWASHER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DISHWASHER")]
        Dishwasher,

        /// <summary>
        /// action.devices.types.DOOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DOOR")]
        Door,

        /// <summary>
        /// action.devices.types.DRAWER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DRAWER")]
        Drawer,

        /// <summary>
        /// action.devices.types.DRYER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.DRYER")]
        Dryer,

        /// <summary>
        /// action.devices.types.FAN.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FAN")]
        Fan,

        /// <summary>
        /// action.devices.types.FAUCET.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FAUCET")]
        Faucet,

        /// <summary>
        /// action.devices.types.FIREPLACE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FIREPLACE")]
        Fireplace,

        /// <summary>
        /// action.devices.types.FREEZER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FREEZER")]
        Freezer,

        /// <summary>
        /// action.devices.types.FRYER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.FRYER")]
        Fryer,

        /// <summary>
        /// action.devices.types.GARAGE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.GARAGE")]
        GarageDoor,

        /// <summary>
        /// action.devices.types.GATE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.GATE")]
        Gate,

        /// <summary>
        /// action.devices.types.GRILL.
        /// </summary>
        [EnumMember(Value = "action.devices.types.GRILL")]
        Grill,

        /// <summary>
        /// action.devices.types.HEATER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.HEATER")]
        Heater,

        /// <summary>
        /// action.devices.types.HOOD.
        /// </summary>
        [EnumMember(Value = "action.devices.types.HOOD")]
        Hood,

        /// <summary>
        /// action.devices.types.HUMIDIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.HUMIDIFIER")]
        Humidifier,

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
        /// action.devices.types.LOCK.
        /// </summary>
        [EnumMember(Value = "action.devices.types.LOCK")]
        Lock,

        /// <summary>
        /// action.devices.types.REMOTECONTROL.
        /// </summary>
        [EnumMember(Value = "action.devices.types.REMOTECONTROL")]
        MediaRemote,

        /// <summary>
        /// action.devices.types.MOP.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MOP")]
        Mop,

        /// <summary>
        /// action.devices.types.MOWER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MOWER")]
        Mower,

        /// <summary>
        /// action.devices.types.MICROWAVE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MICROWAVE")]
        Microwave,

        /// <summary>
        /// action.devices.types.MULTICOOKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.MULTICOOKER")]
        Multicooker,

        /// <summary>
        /// action.devices.types.NETWORK.
        /// </summary>
        [EnumMember(Value = "action.devices.types.NETWORK")]
        Network,

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
        /// action.devices.types.PERGOLA.
        /// </summary>
        [EnumMember(Value = "action.devices.types.PERGOLA")]
        Pergola,

        /// <summary>
        /// action.devices.types.PETFEEDER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.PETFEEDER")]
        PetFeeder,

        /// <summary>
        /// action.devices.types.PRESSURECOOKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.PRESSURECOOKER")]
        PressureCooker,

        /// <summary>
        /// action.devices.types.RADIATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.RADIATOR")]
        Radiator,

        /// <summary>
        /// action.devices.types.REFRIGERATOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.REFRIGERATOR")]
        Refrigerator,

        /// <summary>
        /// action.devices.types.ROUTER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.ROUTER")]
        Router,

        /// <summary>
        /// action.devices.types.SCENE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SCENE")]
        Scene,

        /// <summary>
        /// action.devices.types.SENSOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SENSOR")]
        Sensor,

        /// <summary>
        /// action.devices.types.SECURITYSYSTEM.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SECURITYSYSTEM")]
        SecuritySystem,

        /// <summary>
        /// action.devices.types.SETTOP.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SETTOP")]
        SetTopBox,

        /// <summary>
        /// action.devices.types.SHUTTER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SHUTTER")]
        Shutter,

        /// <summary>
        /// action.devices.types.SHOWER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SHOWER")]
        Shower,

        /// <summary>
        /// action.devices.types.SMOKE_DETECTOR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SMOKE_DETECTOR")]
        SmokeDetector,

        /// <summary>
        /// action.devices.types.SOUSVIDE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SOUSVIDE")]
        SousVide,

        /// <summary>
        /// action.devices.types.SPEAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SPEAKER")]
        Speaker,

        /// <summary>
        /// action.devices.types.STREAMING_BOX.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STREAMING_BOX")]
        StreamingBox,

        /// <summary>
        /// action.devices.types.STREAMING_STICK.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STREAMING_STICK")]
        StreamingStick,

        /// <summary>
        /// action.devices.types.STREAMING_SOUNDBAR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STREAMING_SOUNDBAR")]
        StreamingSoundbar,

        /// <summary>
        /// action.devices.types.SOUNDBAR.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SOUNDBAR")]
        Soundbar,

        /// <summary>
        /// action.devices.types.SPRINKLER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SPRINKLER")]
        Sprinkler,

        /// <summary>
        /// action.devices.types.STANDMIXER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.STANDMIXER")]
        StandMixer,

        /// <summary>
        /// action.devices.types.SWITCH.
        /// </summary>
        [EnumMember(Value = "action.devices.types.SWITCH")]
        Switch,

        /// <summary>
        /// action.devices.types.TV.
        /// </summary>
        [EnumMember(Value = "action.devices.types.TV")]
        Television,

        /// <summary>
        /// action.devices.types.THERMOSTAT.
        /// </summary>
        [EnumMember(Value = "action.devices.types.THERMOSTAT")]
        Thermostat,

        /// <summary>
        /// action.devices.types.VACUUM.
        /// </summary>
        [EnumMember(Value = "action.devices.types.VACUUM")]
        Vacuum,

        /// <summary>
        /// action.devices.types.VALVE.
        /// </summary>
        [EnumMember(Value = "action.devices.types.VALVE")]
        Valve,

        /// <summary>
        /// action.devices.types.WASHER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WASHER")]
        Washer,

        /// <summary>
        /// action.devices.types.WATERHEATER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WATERHEATER")]
        WaterHeater,

        /// <summary>
        /// action.devices.types.WATERPURIFIER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WATERPURIFIER")]
        WaterPurifier,

        /// <summary>
        /// action.devices.types.WATERSOFTENER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WATERSOFTENER")]
        WaterSoftener,

        /// <summary>
        /// action.devices.types.WINDOW.
        /// </summary>
        [EnumMember(Value = "action.devices.types.WINDOW")]
        Window,

        /// <summary>
        /// action.devices.types.YOGURTMAKER.
        /// </summary>
        [EnumMember(Value = "action.devices.types.YOGURTMAKER")]
        YogurtMaker,

        /// <summary>
        /// Unknown device type.
        /// </summary>
        Unknown
    }
}