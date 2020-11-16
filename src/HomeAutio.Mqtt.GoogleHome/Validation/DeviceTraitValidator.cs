using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Models.State.Challenges;
using Newtonsoft.Json;

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
        /// <returns>Validation errors.</returns>
        public static IEnumerable<string> Validate(DeviceTrait deviceTrait)
        {
            var validationErrors = new List<string>();

            var validationModel = SchemaValidationProvider.GetTraitSchemas().GetAwaiter().GetResult();
            var traitName = deviceTrait.Trait.ToEnumString();
            if (validationModel.ContainsKey(traitName))
            {
                // Attribute validation
                if (deviceTrait.Attributes != null && validationErrors != null)
                {
                    var attributeJson = JsonConvert.SerializeObject(deviceTrait.Attributes);
                    var attributeValidator = validationModel[traitName].AttributesSchema;
                    var attributeErrors = attributeValidator.Validate(attributeJson);

                    validationErrors.AddRange(attributeErrors.Select(x => $"{x.Path}: {x.Kind}"));
                }

                // State validation
                if (deviceTrait.State != null && validationModel[traitName].StatesSchema != null)
                {
                    var stateJson = JsonConvert.SerializeObject(GetGoogleState(deviceTrait.State));
                    var stateValidator = validationModel[traitName].StatesSchema;
                    var stateErrors = stateValidator.Validate(stateJson);

                    validationErrors.AddRange(stateErrors.Select(x => $"{x.Path}: {x.Kind}"));
                }

                // Command validations
                var deviceCommands = deviceTrait.Commands.ToDictionary(
                    k => k.Key,
                    v => v.Value.ToDictionary(
                        x => x.Key,
                        x => (object)x.Value).ToNestedDictionary());

                foreach (var command in deviceCommands)
                {
                    if (validationModel[traitName].CommandSchemas.ContainsKey(command.Key))
                    {
                        var commandValidator = validationModel[traitName].CommandSchemas[command.Key];

                        // Modify the schema validation, only looking for presence not type, etc. matching
                        ChangeLeafNodesToString(commandValidator);

                        var commandJson = JsonConvert.SerializeObject(command.Value);
                        var commandErrors = commandValidator.Validate(commandJson);

                        validationErrors.AddRange(commandErrors.Select(x => $"{x.Path}: {x.Kind}"));
                    }
                }
            }

            return validationErrors;
        }

        private static void ChangeLeafNodesToString(NJsonSchema.JsonSchema schema)
        {
            foreach (var property in schema.Properties)
            {
                switch (property.Value.Type)
                {
                    case NJsonSchema.JsonObjectType.String:
                        // Do nothing
                        break;
                    case NJsonSchema.JsonObjectType.Array:
                    case NJsonSchema.JsonObjectType.Object:
                        ChangeLeafNodesToString(property.Value);
                        break;
                    default:
                        property.Value.Type = NJsonSchema.JsonObjectType.String;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets device state as a Google device state object in a flattened state.
        /// </summary>
        /// <param name="stateConfigs">Current state cache.</param>
        /// <returns>A Google device state object in a flattened state.</returns>
        private static IDictionary<string, object> GetGoogleState(IDictionary<string, DeviceState> stateConfigs)
        {
            var stateValues = new Dictionary<string, object>();
            foreach (var state in stateConfigs)
            {
                if (state.Value.Topic != null)
                {
                    switch (state.Value.GoogleType)
                    {
                        case GoogleType.Bool:
                            stateValues.Add(state.Key, state.Value.MapValueToGoogle("true"));
                            break;
                        case GoogleType.Numeric:
                            stateValues.Add(state.Key, state.Value.MapValueToGoogle("1"));
                            break;
                        case GoogleType.String:
                        default:
                            stateValues.Add(state.Key, state.Value.MapValueToGoogle("default"));
                            break;
                    }
                }
            }

            var filteredStateValues = new Dictionary<string, object>();
            foreach (var key in stateValues.Keys)
            {
                switch (key)
                {
                    case "color.spectrumRGB":
                        filteredStateValues.Add("color.spectrumRgb", stateValues[key]);
                        break;
                    case "color.spectrumHSV.hue":
                        filteredStateValues.Add("color.spectrumHsv.hue", stateValues[key]);
                        break;
                    case "color.spectrumHSV.saturation":
                        filteredStateValues.Add("color.spectrumHsv.saturation", stateValues[key]);
                        break;
                    case "color.spectrumHSV.value":
                        filteredStateValues.Add("color.spectrumHsv.value", stateValues[key]);
                        break;
                    default:
                        filteredStateValues.Add(key, stateValues[key]);
                        break;
                }
            }

            return filteredStateValues.ToNestedDictionary();
        }

        /// <summary>
        /// Validates a <see cref="DeviceTrait"/>.
        /// </summary>
        /// <param name="deviceTrait">The <see cref="DeviceTrait"/> to validate.</param>
        /// <returns>Validation errors.</returns>
        public static IEnumerable<string> ValidateOld(DeviceTrait deviceTrait)
        {
            var validationErrors = new List<string>();

            if (deviceTrait.Trait == TraitType.Unknown)
                validationErrors.Add("Trait is missing or not a valid type");

            if (deviceTrait.Challenge != null)
            {
                var pinChallenge = deviceTrait.Challenge as PinChallenge;
                if (pinChallenge != null && string.IsNullOrWhiteSpace(pinChallenge.Pin))
                    validationErrors.Add("Trait pin challenge is missing pin");
            }

            switch (deviceTrait.Trait)
            {
                case TraitType.Brightness:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.BrightnessAbsolute,
                        new List<string> { "brightness" },
                        new List<string> { "brightness" }));
                    break;
                case TraitType.CameraStream:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.GetCameraStream,
                        null,
                        new List<string> { "cameraStreamAccessUrl" },
                        new List<string> { "cameraStreamSupportedProtocols", "cameraStreamNeedAuthToken", "cameraStreamNeedDrmEncryption" }));
                    break;
                case TraitType.Channel:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.SelectChannel,
                        new List<string> { "channelNumber" },
                        null,
                        new List<string> { "availableChannels" }));
                    break;
                case TraitType.ColorSetting:
                    validationErrors.AddRange(ValidateColorSetting(deviceTrait));
                    break;
                case TraitType.Dock:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.Dock,
                        null,
                        new List<string> { "isDocked" },
                        null));
                    break;
                case TraitType.FanSpeed:
                    if (deviceTrait.Attributes.ContainsKey("reversible") && (bool)deviceTrait.Attributes["reversible"])
                    {
                        validationErrors.AddRange(ValidateTrait(
                            deviceTrait,
                            CommandType.Reverse,
                            null,
                            null,
                            new List<string> { "reversible" }));
                    }

                    if (deviceTrait.Attributes.ContainsKey("supportsFanSpeedPercent"))
                    {
                        validationErrors.AddRange(ValidateTrait(
                            deviceTrait,
                            CommandType.SetFanSpeed,
                            new List<string> { "fanSpeedPercent" },
                            new List<string> { "currentFanSpeedPercent" },
                            new List<string> { "supportsFanSpeedPercent.*" }));
                    }

                    if (deviceTrait.Attributes.ContainsKey("availableFanSpeeds"))
                    {
                        validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                            CommandType.SetFanSpeed,
                            new List<string> { "fanSpeed" },
                            new List<string> { "currentFanSpeedSetting" },
                            new List<string> { "availableFanSpeeds.*" }));
                    }

                    break;
                case TraitType.Locator:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.Locate,
                        null,
                        null,
                        null));
                    break;
                case TraitType.Modes:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.SetModes,
                        new List<string> { "updateModeSettings.*" },
                        new List<string> { "currentModeSettings.*" },
                        new List<string> { "availableModes" }));
                    break;
                case TraitType.OnOff:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.OnOff,
                        new List<string> { "on" },
                        new List<string> { "on" }));
                    break;
                case TraitType.OpenClose:
                    if (deviceTrait.Attributes.ContainsKey("queryOnlyOpenClose") && (bool)deviceTrait.Attributes["queryOnlyOpenClose"])
                    {
                        validationErrors.AddRange(ValidateTrait(
                            deviceTrait,
                            CommandType.Unknown,
                            null,
                            new List<string> { "openPercent" },
                            new List<string> { "queryOnlyOpenClose" }));
                    }
                    else
                    {
                        validationErrors.AddRange(ValidateTrait(
                            deviceTrait,
                            CommandType.OpenClose,
                            new List<string> { "openPercent" },
                            new List<string> { "openPercent" }));
                    }

                    break;
                case TraitType.RunCycle:
                    validationErrors.AddRange(ValidateRunCycle(deviceTrait));
                    break;
                case TraitType.Scene:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.ActivateScene,
                        new List<string> { "deactivate" },
                        null,
                        new List<string> { "sceneReversible" }));
                    break;
                case TraitType.StartStop:
                    if (deviceTrait.Attributes.ContainsKey("pausable") && (bool)deviceTrait.Attributes["pausable"])
                    {
                        validationErrors.AddRange(ValidateTrait(
                            deviceTrait,
                            CommandType.PauseUnpause,
                            new List<string> { "pause" },
                            new List<string> { "isPaused" },
                            new List<string> { "pausable" }));
                    }

                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.StartStop,
                        new List<string> { "start" },
                        new List<string> { "isRunning" },
                        null));
                    break;
                case TraitType.TemperatureControl:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.SetTemperature,
                        new List<string> { "temperature" },
                        new List<string> { "temperatureSetpointCelsius", "temperatureAmbientCelsius" },
                        new List<string> { "temperatureRange.*", "temperatureUnitForUX" }));
                    break;
                case TraitType.TemperatureSetting:
                    validationErrors.AddRange(ValidateTemperatureSetting(deviceTrait));
                    break;
                case TraitType.Toggles:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.SetToggles,
                        new List<string> { "updateToggleSettings.*" },
                        new List<string> { "currentToggleSettings.*" },
                        new List<string> { "availableToggles" }));
                    break;
                case TraitType.Volume:
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        CommandType.SetVolume,
                        new List<string> { "volumeLevel" },
                        new List<string> { "currentVolume", "isMuted" },
                        null));
                    break;
            }

            return validationErrors;
        }

        /// <summary>
        /// Validates a ColorAbsolute trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        /// <returns>Validation errors.</returns>
        private static IEnumerable<string> ValidateColorSetting(DeviceTrait deviceTrait)
        {
            var command = CommandType.ColorAbsolute;
            var commandParams = new List<string>();
            var stateKeys = new List<string>();
            var attributeKeys = new List<string>();

            if (deviceTrait.Attributes.ContainsKey("colorTemperatureRange"))
            {
                // Temperature range
                attributeKeys.AddRange(new List<string> { "colorTemperatureRange.temperatureMinK", "colorTemperatureRange.temperatureMaxK" });
                commandParams.AddRange(new List<string> { "color.temperature" });
                stateKeys.AddRange(new List<string> { "color.temperatureK" });
            }

            if (deviceTrait.Attributes.ContainsKey("colorModel") && (string)deviceTrait.Attributes["colorModel"] == "hsv")
            {
                // HSV requirements
                commandParams.AddRange(new List<string> { "color.spectrumHSV.hue", "color.spectrumHSV.saturation", "color.spectrumHSV.value" });
                stateKeys.AddRange(new List<string> { "color.spectrumHsv.hue", "color.spectrumHsv.saturation", "color.spectrumHsv.value" });
            }
            else if (deviceTrait.Attributes.ContainsKey("colorModel") && (string)deviceTrait.Attributes["colorModel"] == "rgb")
            {
                // RGB requirements
                commandParams.AddRange(new List<string> { "color.spectrumRGB" });
                stateKeys.AddRange(new List<string> { "color.spectrumRgb" });
            }

            if (deviceTrait.Attributes.ContainsKey("commandOnlyColorSetting") && (bool)deviceTrait.Attributes["commandOnlyColorSetting"] == true)
            {
                // Command checks only, wipe expeced states
                stateKeys = null;
            }

            return ValidateTrait(deviceTrait, command, commandParams, stateKeys, attributeKeys);
        }

        /// <summary>
        /// Validates a RunCycle trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        /// <returns>Validation errors.</returns>
        private static IEnumerable<string> ValidateRunCycle(DeviceTrait deviceTrait)
        {
            var validationErrors = new List<string>();

            var stateKeys = new List<string> { "currentRunCycle", "currentTotalRemainingTime", "currentCycleRemainingTime" };
            foreach (var stateKey in stateKeys)
            {
                if (!deviceTrait.State.ContainsKey(stateKey))
                    validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing state '{stateKey}'");
            }

            return validationErrors;
        }

        /// <summary>
        /// Validates a RunCycle trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        /// <returns>Validation errors.</returns>
        private static IEnumerable<string> ValidateTemperatureSetting(DeviceTrait deviceTrait)
        {
            var validationErrors = new List<string>();

            // Command only mode
            var commandOnlyTemperatureSetting = deviceTrait.Attributes.ContainsKey("commandOnlyTemperatureSetting")
                ? (bool)deviceTrait.Attributes["commandOnlyTemperatureSetting"]
                : false;

            // Query only mode
            var queryOnlyTemperatureSetting = deviceTrait.Attributes.ContainsKey("queryOnlyTemperatureSetting")
                ? (bool)deviceTrait.Attributes["queryOnlyTemperatureSetting"]
                : false;

            // ThermostatSetMode
            if (deviceTrait.Commands.ContainsKey(CommandType.ThermostatSetMode.ToEnumString()))
            {
                var command = queryOnlyTemperatureSetting ? CommandType.Unknown : CommandType.ThermostatSetMode;
                var commandParams = queryOnlyTemperatureSetting ? null : new List<string> { "thermostatMode" };
                var stateKeys = commandOnlyTemperatureSetting ? null : new List<string> { "thermostatMode" };
                validationErrors.AddRange(ValidateTrait(
                    deviceTrait,
                    command,
                    commandParams,
                    stateKeys,
                    new List<string> { "availableThermostatModes" }));
            }

            // ThermostatTemperatureSetpoint
            if (deviceTrait.Commands.ContainsKey(CommandType.ThermostatTemperatureSetpoint.ToEnumString()))
            {
                var command = queryOnlyTemperatureSetting ? CommandType.Unknown : CommandType.ThermostatTemperatureSetpoint;
                var commandParams = queryOnlyTemperatureSetting ? null : new List<string> { "thermostatTemperatureSetpoint" };
                var stateKeys = commandOnlyTemperatureSetting ? null : new List<string> { "thermostatTemperatureSetpoint" };
                validationErrors.AddRange(ValidateTrait(
                    deviceTrait,
                    command,
                    commandParams,
                    stateKeys,
                    new List<string> { "thermostatTemperatureUnit" }));
            }

            // ThermostatTemperatureSetRange
            if (deviceTrait.Commands.ContainsKey(CommandType.ThermostatTemperatureSetRange.ToEnumString()))
            {
                // Only supported if heatcool mode is supported
                var availableThermostatModes = ((List<object>)deviceTrait.Attributes["availableThermostatModes"]).Cast<string>();
                if (availableThermostatModes.Contains("heatcool"))
                {
                    var command = queryOnlyTemperatureSetting ? CommandType.Unknown : CommandType.ThermostatTemperatureSetRange;
                    var commandParams = queryOnlyTemperatureSetting ? null : new List<string> { "thermostatTemperatureSetpointHigh", "thermostatTemperatureSetpointLow" };
                    var stateKeys = commandOnlyTemperatureSetting ? null : new List<string> { "thermostatTemperatureSetpointHigh", "thermostatTemperatureSetpointLow" };
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        command,
                        commandParams,
                        stateKeys,
                        new List<string> { "thermostatTemperatureUnit" }));
                }
            }

            // TemperatureRelative
            var temperatureRelativeCommandName = CommandType.TemperatureRelative.ToEnumString();
            if (commandOnlyTemperatureSetting && deviceTrait.Commands.ContainsKey(temperatureRelativeCommandName))
            {
                // This command is only available if the commandOnlyTemperatureSetting attribute of the device is set to true. Only one of the following parameters will be set:
                var command = queryOnlyTemperatureSetting ? CommandType.Unknown : CommandType.TemperatureRelative;
                if (!deviceTrait.Commands[temperatureRelativeCommandName].ContainsKey("thermostatTemperatureRelativeDegree"))
                {
                    var commandParams = queryOnlyTemperatureSetting ? null : new List<string> { "thermostatTemperatureRelativeDegree" };
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        command,
                        commandParams,
                        null,
                        new List<string> { "commandOnlyTemperatureSetting" }));
                }
                else
                {
                    var commandParams = queryOnlyTemperatureSetting ? null : new List<string> { "thermostatTemperatureRelativeWeight" };
                    validationErrors.AddRange(ValidateTrait(
                        deviceTrait,
                        command,
                        commandParams,
                        null,
                        new List<string> { "commandOnlyTemperatureSetting" }));
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Validates a trait.
        /// </summary>
        /// <param name="deviceTrait">Device trait to validate.</param>
        /// <param name="command">Command to verify.</param>
        /// <param name="commandParams">Command params expected for command.</param>
        /// <param name="stateKeys">State keys expected for command.</param>
        /// <param name="attributeKeys">Attribute keys expected for command.</param>
        /// <returns>Validation errors.</returns>
        private static IEnumerable<string> ValidateTrait(
            DeviceTrait deviceTrait,
            CommandType command,
            IEnumerable<string> commandParams,
            IEnumerable<string> stateKeys,
            IEnumerable<string> attributeKeys = null)
        {
            var validationErrors = new List<string>();

            var commandName = command.ToEnumString();
            if (command != CommandType.Unknown && !deviceTrait.Commands.ContainsKey(commandName))
                validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing required command '{commandName}'");

            if (commandParams != null)
            {
                foreach (var commandParam in commandParams)
                {
                    if (commandParam.EndsWith(".*"))
                    {
                        if (!deviceTrait.Commands[commandName].Keys.Any(x => x.StartsWith(commandParam.Substring(0, commandParam.Length - 2))))
                            validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing command param '{commandParam}' for command '{commandName}'");
                    }
                    else
                    {
                        if (!deviceTrait.Commands[commandName].ContainsKey(commandParam))
                            validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing command param '{commandParam}' for command '{commandName}'");
                    }
                }
            }

            if (stateKeys != null)
            {
                foreach (var stateKey in stateKeys)
                {
                    if (stateKey.EndsWith(".*"))
                    {
                        if (!deviceTrait.State.Keys.Any(x => x.StartsWith(stateKey.Substring(0, stateKey.Length - 2))))
                            validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing state '{stateKey}' for command '{commandName}'");
                    }
                    else
                    {
                        if (!deviceTrait.State.ContainsKey(stateKey))
                            validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing state '{stateKey}' for command '{commandName}'");
                    }
                }
            }

            if (attributeKeys != null)
            {
                var flattenedAttributes = deviceTrait.Attributes != null
                    ? deviceTrait.Attributes.ToFlatDictionary()
                    : new Dictionary<string, object>();
                foreach (var attributeKey in attributeKeys)
                {
                    if (attributeKey.EndsWith(".*"))
                    {
                        if (!flattenedAttributes.Keys.Any(x => x.StartsWith(attributeKey.Substring(0, attributeKey.Length - 2))))
                            validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing attribute '{attributeKey}' for command '{commandName}'");
                    }
                    else
                    {
                        if (!flattenedAttributes.ContainsKey(attributeKey))
                            validationErrors.Add($"Trait '{deviceTrait.Trait}' is missing attribute '{attributeKey}' for command '{commandName}'");
                    }
                }
            }

            return validationErrors;
        }
    }
}
