using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Trait type enumeration.
    /// </summary>
    public enum TraitType
    {
        /// <summary>
        /// Unknown trait type.
        /// </summary>
        Unknown,

        /// <summary>
        /// action.devices.traits.AppSelector.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.AppSelector")]
        AppSelector,

        /// <summary>
        /// action.devices.traits.ArmDisarm.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.ArmDisarm")]
        ArmDisarm,

        /// <summary>
        /// action.devices.traits.Brightness.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Brightness")]
        Brightness,

        /// <summary>
        /// action.devices.traits.CameraStream.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.CameraStream")]
        CameraStream,

        /// <summary>
        /// action.devices.traits.Channel.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Channel")]
        Channel,

        /// <summary>
        /// action.devices.traits.ColorSetting.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.ColorSetting")]
        ColorSetting,

        /// <summary>
        /// action.devices.traits.Cook.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Cook")]
        Cook,

        /// <summary>
        /// action.devices.traits.Dispense.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Dispense")]
        Dispense,

        /// <summary>
        /// action.devices.traits.Dock.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Dock")]
        Dock,

        /// <summary>
        /// action.devices.traits.EnergyStorage.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.EnergyStorage")]
        EnergyStorage,

        /// <summary>
        /// action.devices.traits.FanSpeed.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.FanSpeed")]
        FanSpeed,

        /// <summary>
        /// action.devices.traits.Fill.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Fill")]
        Fill,

        /// <summary>
        /// action.devices.traits.HumiditySetting.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.HumiditySetting")]
        HumiditySetting,

        /// <summary>
        /// action.devices.traits.InputSelector.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.InputSelector")]
        InputSelector,

        /// <summary>
        /// action.devices.traits.LightEffects.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.LightEffects")]
        LightEffects,

        /// <summary>
        /// action.devices.traits.Locator.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Locator")]
        Locator,

        /// <summary>
        /// action.devices.traits.LockUnlock.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.LockUnlock")]
        LockUnlock,

        /// <summary>
        /// action.devices.traits.MediaState.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.MediaState")]
        MediaState,

        /// <summary>
        /// action.devices.traits.Modes.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Modes")]
        Modes,

        /// <summary>
        /// action.devices.traits.NetworkControl.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.NetworkControl")]
        NetworkControl,

        /// <summary>
        /// action.devices.traits.ObjectDetection.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.ObjectDetection")]
        ObjectDetection,

        /// <summary>
        /// action.devices.traits.OnOff.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.OnOff")]
        OnOff,

        /// <summary>
        /// action.devices.traits.OpenClose.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.OpenClose")]
        OpenClose,

        /// <summary>
        /// action.devices.traits.Reboot.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Reboot")]
        Reboot,

        /// <summary>
        /// action.devices.traits.Rotation.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Rotation")]
        Rotation,

        /// <summary>
        /// action.devices.traits.RunCycle.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.RunCycle")]
        RunCycle,

        /// <summary>
        /// action.devices.traits.Scene.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Scene")]
        Scene,

        /// <summary>
        /// action.devices.traits.SensorState.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.SensorState")]
        SensorState,

        /// <summary>
        /// action.devices.traits.SoftwareUpdate.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.SoftwareUpdate")]
        SoftwareUpdate,

        /// <summary>
        /// action.devices.traits.StartStop.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.StartStop")]
        StartStop,

        /// <summary>
        /// action.devices.traits.StatusReport.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.StatusReport")]
        StatusReport,

        /// <summary>
        /// action.devices.traits.TemperatureControl.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.TemperatureControl")]
        TemperatureControl,

        /// <summary>
        /// action.devices.traits.TemperatureSetting.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.TemperatureSetting")]
        TemperatureSetting,

        /// <summary>
        /// action.devices.traits.Timer.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Timer")]
        Timer,

        /// <summary>
        /// action.devices.traits.Toggles.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Toggles")]
        Toggles,

        /// <summary>
        /// action.devices.traits.TransportControl.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.TransportControl")]
        TransportControl,

        /// <summary>
        /// action.devices.traits.Volume.
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Volume")]
        Volume
    }
}
