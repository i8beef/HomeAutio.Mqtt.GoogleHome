using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device command types.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Unknown command type.
        /// </summary>
        [EnumMember(Value = "Unknown")]
        Unknown,

        /// <summary>
        /// action.devices.commands.ActivateScene
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ActivateScene")]
        ActivateScene,

        /// <summary>
        /// action.devices.commands.appInstall
        /// </summary>
        [EnumMember(Value = "action.devices.commands.appInstall")]
        AppInstall,

        /// <summary>
        /// action.devices.commands.appSearch
        /// </summary>
        [EnumMember(Value = "action.devices.commands.appSearch")]
        AppSearch,

        /// <summary>
        /// action.devices.commands.appSelect
        /// </summary>
        [EnumMember(Value = "action.devices.commands.appSelect")]
        AppSelect,

        /// <summary>
        /// action.devices.commands.ArmDisarm
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ArmDisarm")]
        ArmDisarm,

        /// <summary>
        /// action.devices.commands.BrightnessAbsolute
        /// </summary>
        [EnumMember(Value = "action.devices.commands.BrightnessAbsolute")]
        BrightnessAbsolute,

        /// <summary>
        /// action.devices.commands.BrightnessRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.BrightnessRelative")]
        BrightnessRelative,

        /// <summary>
        /// action.devices.commands.Charge
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Charge")]
        Charge,

        /// <summary>
        /// action.devices.commands.ColorAbsolute
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ColorAbsolute")]
        ColorAbsolute,

        /// <summary>
        /// action.devices.commands.ColorLoop
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ColorLoop")]
        ColorLoop,

        /// <summary>
        /// action.devices.commands.Cook
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Cook")]
        Cook,

        /// <summary>
        /// action.devices.commands.Dispense
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Dispense")]
        Dispense,

        /// <summary>
        /// action.devices.commands.Dock
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Dock")]
        Dock,

        /// <summary>
        /// action.devices.commands.EnableDisableGuestNetwork
        /// </summary>
        [EnumMember(Value = "action.devices.commands.EnableDisableGuestNetwork")]
        EnableDisableGuestNetwork,

        /// <summary>
        /// action.devices.commands.EnableDisableNetworkProfile
        /// </summary>
        [EnumMember(Value = "action.devices.commands.EnableDisableNetworkProfile")]
        EnableDisableNetworkProfile,

        /// <summary>
        /// action.devices.commands.Fill
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Fill")]
        Fill,

        /// <summary>
        /// action.devices.commands.GetCameraStream
        /// </summary>
        [EnumMember(Value = "action.devices.commands.GetCameraStream")]
        GetCameraStream,

        /// <summary>
        /// action.devices.commands.GetGuestNetworkPassword
        /// </summary>
        [EnumMember(Value = "action.devices.commands.GetGuestNetworkPassword")]
        GetGuestNetworkPassword,

        /// <summary>
        /// action.devices.commands.HumidityRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.HumidityRelative")]
        HumidityRelative,

        /// <summary>
        /// action.devices.commands.Locate
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Locate")]
        Locate,

        /// <summary>
        /// action.devices.commands.LockUnlock
        /// </summary>
        [EnumMember(Value = "action.devices.commands.LockUnlock")]
        LockUnlock,

        /// <summary>
        /// action.devices.commands.mediaClosedCaptioningOff
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaClosedCaptioningOff")]
        MediaClosedCaptioningOff,

        /// <summary>
        /// action.devices.commands.mediaClosedCaptioningOn
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaClosedCaptioningOn")]
        MediaClosedCaptioningOn,

        /// <summary>
        /// action.devices.commands.mediaNext
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaNext")]
        MediaNext,

        /// <summary>
        /// action.devices.commands.mediaPause
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaPause")]
        MediaPause,

        /// <summary>
        /// action.devices.commands.mediaPrevious
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaPrevious")]
        MediaPrevious,

        /// <summary>
        /// action.devices.commands.mediaRepeatMode
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaRepeatMode")]
        MediaRepeatMode,

        /// <summary>
        /// action.devices.commands.mediaResume
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaResume")]
        MediaResume,

        /// <summary>
        /// action.devices.commands.mediaSeekRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaSeekRelative")]
        MediaSeekRelative,

        /// <summary>
        /// action.devices.commands.mediaSeekToPosition
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaSeekToPosition")]
        MediaSeekToPosition,

        /// <summary>
        /// action.devices.commands.mediaShuffle
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaShuffle")]
        MediaShuffle,

        /// <summary>
        /// action.devices.commands.mediaStop
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mediaStop")]
        MediaStop,

        /// <summary>
        /// action.devices.commands.mute
        /// </summary>
        [EnumMember(Value = "action.devices.commands.mute")]
        Mute,

        /// <summary>
        /// action.devices.commands.NextInput
        /// </summary>
        [EnumMember(Value = "action.devices.commands.NextInput")]
        NextInput,

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
        /// action.devices.commands.OpenCloseRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.OpenCloseRelative")]
        OpenCloseRelative,

        /// <summary>
        /// action.devices.commands.PauseUnpause
        /// </summary>
        [EnumMember(Value = "action.devices.commands.PauseUnpause")]
        PauseUnpause,

        /// <summary>
        /// action.devices.commands.PreviousInput
        /// </summary>
        [EnumMember(Value = "action.devices.commands.PreviousInput")]
        PreviousInput,

        /// <summary>
        /// action.devices.commands.Reboot
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Reboot")]
        Reboot,

        /// <summary>
        /// action.devices.commands.relativeChannel
        /// </summary>
        [EnumMember(Value = "action.devices.commands.relativeChannel")]
        RelativeChannel,

        /// <summary>
        /// action.devices.commands.returnChannel
        /// </summary>
        [EnumMember(Value = "action.devices.commands.returnChannel")]
        ReturnChannel,

        /// <summary>
        /// action.devices.commands.Reverse
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Reverse")]
        Reverse,

        /// <summary>
        /// action.devices.commands.RotateAbsolute
        /// </summary>
        [EnumMember(Value = "action.devices.commands.RotateAbsolute")]
        RotateAbsolute,

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
        /// action.devices.commands.SetFanSpeedRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetFanSpeedRelative")]
        SetFanSpeedRelative,

        /// <summary>
        /// action.devices.commands.SetHumidity
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetHumidity")]
        SetHumidity,

        /// <summary>
        /// action.devices.commands.SetInput
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SetInput")]
        SetInput,

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
        /// action.devices.commands.Sleep
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Sleep")]
        Sleep,

        /// <summary>
        /// action.devices.commands.SoftwareUpdate
        /// </summary>
        [EnumMember(Value = "action.devices.commands.SoftwareUpdate")]
        SoftwareUpdate,

        /// <summary>
        /// action.devices.commands.StartStop
        /// </summary>
        [EnumMember(Value = "action.devices.commands.StartStop")]
        StartStop,

        /// <summary>
        /// action.devices.commands.StopEffect
        /// </summary>
        [EnumMember(Value = "action.devices.commands.StopEffect")]
        StopEffect,

        /// <summary>
        /// action.devices.commands.TemperatureRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TemperatureRelative")]
        TemperatureRelative,

        /// <summary>
        /// action.devices.commands.TestNetworkSpeed
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TestNetworkSpeed")]
        TestNetworkSpeed,

        /// <summary>
        /// action.devices.commands.ThermostatSetMode
        /// </summary>
        [EnumMember(Value = "action.devices.commands.ThermostatSetMode")]
        ThermostatSetMode,

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
        /// action.devices.commands.TimerAdjust
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TimerAdjust")]
        TimerAdjust,

        /// <summary>
        /// action.devices.commands.TimerCancel
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TimerCancel")]
        TimerCancel,

        /// <summary>
        /// action.devices.commands.TimerPause
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TimerPause")]
        TimerPause,

        /// <summary>
        /// action.devices.commands.TimerResume
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TimerResume")]
        TimerResume,

        /// <summary>
        /// action.devices.commands.TimerStart
        /// </summary>
        [EnumMember(Value = "action.devices.commands.TimerStart")]
        TimerStart,

        /// <summary>
        /// action.devices.commands.volumeRelative
        /// </summary>
        [EnumMember(Value = "action.devices.commands.volumeRelative")]
        VolumeRelative,

        /// <summary>
        /// action.devices.commands.Wake
        /// </summary>
        [EnumMember(Value = "action.devices.commands.Wake")]
        Wake
    }
}
