using System;
using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// <see cref="DeviceTrait"/> validator.
    /// </summary>
    public static class DeviceTraitValidator
    {
        /// <summary>
        /// Validates a <see cref="DeviceTrait"/>.
        /// </summary>
        /// <param name="deviceTrait">The <see cref="DeviceTrait"/> to validate.</param>
        public static void Validate(DeviceTrait deviceTrait)
        {
            if (deviceTrait.Trait == TraitType.Unknown)
                throw new Exception("Trait is missing or not a valid type");

            switch (deviceTrait.Trait)
            {
                case TraitType.Brightness:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.BrightnessAbsolute,
                        new List<string> { "brightness" },
                        new List<string> { "brightness" });
                    break;
                case TraitType.CameraStream:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.CameraStream,
                        null,
                        new List<string> { "cameraStreamAccessUrl" },
                        new List<string> { "cameraStreamSupportedProtocols", "cameraStreamNeedAuthToken", "cameraStreamNeedDrmEncryption" });
                    break;
                case TraitType.ColorSetting:
                    ValidateColorAbsolute(deviceTrait);
                    break;
                case TraitType.ColorSpectrum:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.ColorAbsolute,
                        new List<string> { "spectrumRGB" },
                        new List<string> { "spectrumRgb" },
                        null);
                    break;
                case TraitType.ColorTemperature:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.ColorAbsolute,
                        new List<string> { "temperature" },
                        new List<string> { "temperatureK" },
                        new List<string> { "temperatureMinK", "temperatureMaxK" });
                    break;
                case TraitType.Dock:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.Dock,
                        null,
                        new List<string> { "isDocked" },
                        null);
                    break;
                case TraitType.FanSpeed:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.SetFanSpeed,
                        new List<string> { "fanSpeed" },
                        new List<string> { "currentFanSpeedSetting" },
                        new List<string> { "availableFanSpeeds" });
                    ValidateTrait(
                        deviceTrait,
                        CommandType.Reverse,
                        null,
                        null,
                        new List<string> { "reversible" });
                    break;
                case TraitType.Locator:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.Locate,
                        new List<string> { "silent" },
                        new List<string> { "generatedAlert" },
                        null);
                    break;
                case TraitType.Modes:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.SetModes,
                        new List<string> { "updateModeSettings" },
                        new List<string> { "currentModeSettings" },
                        new List<string> { "availableModes", "ordered" });
                    break;
                case TraitType.OnOff:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.OnOff,
                        new List<string> { "on" },
                        new List<string> { "on" });
                    break;
                case TraitType.RunCycle:
                    ValidateRunCycle(deviceTrait);
                    break;
                case TraitType.Scene:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.ActivateScene,
                        new List<string> { "deactivate" },
                        null,
                        new List<string> { "sceneReversible" });
                    break;
                case TraitType.StartStop:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.SetToggles,
                        new List<string> { "updateToggleSettings" },
                        new List<string> { "currentToggleSettings" },
                        new List<string> { "availableToggles" });
                    break;
                case TraitType.TemperatureControl:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.SetTemperature,
                        new List<string> { "temperature" },
                        new List<string> { "temperatureSetpointCelsius", "temperatureAmbientCelsius" },
                        new List<string> { "temperatureRange", "temperatureUnitForUX" });
                    break;
                case TraitType.TemperatureSetting:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.ThermostatSetMode,
                        new List<string> { "thermostatMode" },
                        new List<string> { "thermostatMode" },
                        new List<string> { "availableThermostatModes" });
                    ValidateTrait(
                        deviceTrait,
                        CommandType.ThermostatTemperatureSetpoint,
                        new List<string> { "thermostatTemperatureSetpointHigh", "thermostatTemperatureSetpointLow" },
                        new List<string> { "thermostatTemperatureSetpointHigh", "thermostatTemperatureSetpointLow" },
                        new List<string> { "thermostatTemperatureUnit" });
                    ValidateTrait(
                        deviceTrait,
                        CommandType.ThermostatTemperatureSetRange,
                        new List<string> { "thermostatTemperatureSetpoint" },
                        new List<string> { "thermostatTemperatureSetpoint" },
                        new List<string> { "thermostatTemperatureUnit" });
                    break;
                case TraitType.Toggles:
                    ValidateTrait(
                        deviceTrait,
                        CommandType.SetToggles,
                        new List<string> { "updateToggleSettings" },
                        new List<string> { "currentToggleSettings" },
                        new List<string> { "availableToggles" });
                    break;
            }
        }

        /// <summary>
        /// Validates a ColorAbsolute trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        private static void ValidateColorAbsolute(DeviceTrait deviceTrait)
        {
            var command = CommandType.ColorAbsolute;
            var commandParams = new List<string>();
            var stateKeys = new List<string>();
            var attributeKeys = new List<string> { "colorModel", "colorTemperatureRange" };

            if (deviceTrait.Attributes.ContainsKey("colorTemperatureRange"))
            {
                // Temperature range
                commandParams.AddRange(new List<string> { "color.temperature " });
                stateKeys.AddRange(new List<string> { "color.temperatureK" });
            }

            if (deviceTrait.Attributes.ContainsKey("colorModel") && (string)deviceTrait.Attributes["colorModel"] == "hsv")
            {
                // HSV requirements
                commandParams.AddRange(new List<string> { "color.spectrumHSV.hue", "color.spectrumHSV.saturation", "color.spectrumHSV.value" });
                stateKeys.AddRange(new List<string> { "color.spectrumHSV.hue", "color.spectrumHSV.saturation", "color.spectrumHSV.value" });
            }
            else
            {
                // RGB is default
                commandParams.AddRange(new List<string> { "color.spectrumRGB" });
                stateKeys.AddRange(new List<string> { "color.spectrumRgb" });
            }

            if (deviceTrait.Attributes.ContainsKey("commandOnlyColorSetting") && (bool)deviceTrait.Attributes["commandOnlyColorSetting"] == true)
            {
                // Command checks only
                stateKeys = null;
            }

            ValidateTrait(deviceTrait, command, commandParams, stateKeys, attributeKeys);
        }

        /// <summary>
        /// Validates a RunCycle trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        private static void ValidateRunCycle(DeviceTrait deviceTrait)
        {
            var stateKeys = new List<string> { "currentRunCycle", "currentTotalRemainingTime", "currentCycleRemainingTime" };
            foreach (var stateKey in stateKeys)
            {
                if (!deviceTrait.State.ContainsKey(stateKey))
                    throw new Exception($"Trait '{deviceTrait}' is missing state '{stateKey}'");
            }
        }

        /// <summary>
        /// Validates a brightness trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        /// <param name="command">Command to verify.</param>
        /// <param name="commandParams">Command params expected for command.</param>
        /// <param name="stateKeys">State keys expected for command.</param>
        /// <param name="attributeKeys">Attribute keys expected for command.</param>
        private static void ValidateTrait(
            DeviceTrait deviceTrait,
            CommandType command,
            IEnumerable<string> commandParams,
            IEnumerable<string> stateKeys,
            IEnumerable<string> attributeKeys = null)
        {
            var commandName = command.ToEnumString();
            if (!deviceTrait.Commands.ContainsKey(commandName))
                throw new Exception($"Trait '{deviceTrait}' is missing required command '{commandName}'");

            if (commandParams != null)
            {
                foreach (var commandParam in commandParams)
                {
                    if (!deviceTrait.Commands[commandName].ContainsKey(commandParam))
                        throw new Exception($"Trait '{deviceTrait}' is missing command param '{commandParam}' for command '{commandName}'");
                }
            }

            if (stateKeys != null)
            {
                foreach (var stateKey in stateKeys)
                {
                    if (!deviceTrait.State.ContainsKey(stateKey))
                        throw new Exception($"Trait '{deviceTrait}' is missing state '{stateKey}' for command '{commandName}'");
                }
            }

            if (attributeKeys != null)
            {
                foreach (var attributeKey in attributeKeys)
                {
                    if (!deviceTrait.Attributes.ContainsKey(attributeKey))
                        throw new Exception($"Trait '{deviceTrait}' is missing attribute '{attributeKey}' for command '{commandName}'");
                }
            }
        }
    }
}
