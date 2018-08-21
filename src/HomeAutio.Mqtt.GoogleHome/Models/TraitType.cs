using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device trait types.
    /// </summary>
    public enum TraitType
    {
        /// <summary>
        /// action.devices.traits.Brightness
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Brightness")]
        Brightness,

        /// <summary>
        /// action.devices.traits.CameraStream
        /// </summary>
        [EnumMember(Value = "action.devices.traits.CameraStream")]
        CameraStream,

        /// <summary>
        /// action.devices.traits.ColorSpectrum
        /// </summary>
        [EnumMember(Value = "action.devices.traits.ColorSpectrum")]
        ColorSpectrum,

        /// <summary>
        /// action.devices.traits.ColorTemperature
        /// </summary>
        [EnumMember(Value = "action.devices.traits.ColorTemperature")]
        ColorTemperature,

        /// <summary>
        /// action.devices.traits.Dock
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Dock")]
        Dock,

        /// <summary>
        /// action.devices.traits.FanSpeed
        /// </summary>
        [EnumMember(Value = "action.devices.traits.FanSpeed")]
        FanSpeed,

        /// <summary>
        /// action.devices.traits.Locator
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Locator")]
        Locator,

        /// <summary>
        /// action.devices.traits.Modes
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Modes")]
        Modes,

        /// <summary>
        /// action.devices.traits.OnOff
        /// </summary>
        [EnumMember(Value = "action.devices.traits.OnOff")]
        OnOff,

        /// <summary>
        /// action.devices.traits.RunCycle
        /// </summary>
        [EnumMember(Value = "action.devices.traits.RunCycle")]
        RunCycle,

        /// <summary>
        /// action.devices.traits.Scene
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Scene")]
        Scene,

        /// <summary>
        /// action.devices.traits.StartStop
        /// </summary>
        [EnumMember(Value = "action.devices.traits.StartStop")]
        StartStop,

        /// <summary>
        /// action.devices.traits.TemperatureControl
        /// </summary>
        [EnumMember(Value = "action.devices.traits.TemperatureControl")]
        TemperatureControl,

        /// <summary>
        /// action.devices.traits.TemperatureSetting
        /// </summary>
        [EnumMember(Value = "action.devices.traits.TemperatureSetting")]
        TemperatureSetting,

        /// <summary>
        /// action.devices.traits.Toggles
        /// </summary>
        [EnumMember(Value = "action.devices.traits.Toggles")]
        Toggles
    }
}
