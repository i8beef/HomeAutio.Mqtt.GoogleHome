using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device command types.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// action.devices.commands.ActivateScene
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ActivateScene")]
        ActivateScene,

        /// <summary>
        /// action.devices.commands.BrightnessAbsolute
        /// </summary>
        [EnumMember(Value = "action.devices.commands.BrightnessAbsolute")]
        BrightnessAbsolute,

        /// <summary>
        /// action.devices.commands.GetCameraStream
        /// </summary>
        [EnumMember(Value = "action.devices.commands.GetCameraStream")]
        CameraStream,

        /// <summary>
        /// action.devices.commands.ColorAbsolute
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ColorAbsolute")]
        ColorAbsolute,

        /// <summary>
        /// action.devices.commands.Dock
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Dock")]
        Dock,

        /// <summary>
        /// action.devices.commands.Locate
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Locate")]
        Locate,

        /// <summary>
        /// action.devices.commands.OnOff
        /// </summary>
        [EnumMember(Value = "action.devices.commands.OnOff")]
        OnOff,

        /// <summary>
        /// action.devices.commands.OpenClose
        /// </summary>
        [EnumMember(Value = "action.devices.commands.OpenClose")]
        OpenClose,

        /// <summary>
        /// action.devices.commands.PauseUnpause
        /// </summary>
        [EnumMember(Value = "action.devices.commands.PauseUnpause")]
        PauseUnpause,

        /// <summary>
        /// action.devices.commands.relativeChannel
        /// </summary>
        [EnumMember(Value = "action.devices.commands.relativeChannel")]
        RelativeChannel,

        /// <summary>
        /// action.devices.commands.Reverse
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Reverse")]
        Reverse,

        /// <summary>
        /// action.devices.commands.selectChannel
        /// </summary>
        [EnumMember(Value = "action.devices.commands.selectChannel")]
        SelectChannel,

        /// <summary>
        /// action.devices.commands.SetFanSpeed
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetFanSpeed")]
        SetFanSpeed,

        /// <summary>
        /// action.devices.commands.SetModes
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetModes")]
        SetModes,

        /// <summary>
        /// action.devices.commands.SetTemperature
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetTemperature")]
        SetTemperature,

        /// <summary>
        /// action.devices.commands.SetToggles
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetToggles")]
        SetToggles,

        /// <summary>
        /// action.devices.commands.setVolume
        /// </summary>
        [EnumMember(Value = "action.devices.commands.setVolume")]
        SetVolume,

        /// <summary>
        /// action.devices.commands.StartStop
        /// </summary>
        [EnumMember(Value = "action.devices.commands.StartStop")]
        StartStop,

        /// <summary>
        /// action.devices.commands.TemperatureRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TemperatureRelative")]
        TemperatureRelative,

        /// <summary>
        /// action.devices.commands.ThermostatTemperatureSetpoint
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ThermostatTemperatureSetpoint")]
        ThermostatTemperatureSetpoint,

        /// <summary>
        /// action.devices.commands.ThermostatTemperatureSetRange
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ThermostatTemperatureSetRange")]
        ThermostatTemperatureSetRange,

        /// <summary>
        /// action.devices.commands.ThermostatSetMode
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ThermostatSetMode")]
        ThermostatSetMode,

        /// <summary>
        /// action.devices.commands.volumeRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.volumeRelative")]
        VolumeRelative,

        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown
    }
}
