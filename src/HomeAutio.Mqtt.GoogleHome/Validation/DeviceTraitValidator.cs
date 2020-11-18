using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models.State;
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
                    var attributeValidator = validationModel[traitName].AttributeValidator;
                    var attributeErrors = attributeValidator.Validate(attributeJson);

                    validationErrors.AddRange(attributeErrors.Select(x => $"{x.Path}: {x.Kind}"));
                }

                // State validation
                if (deviceTrait.State != null && validationModel[traitName].StateValidator != null)
                {
                    var stateJson = JsonConvert.SerializeObject(GetGoogleState(deviceTrait.State));
                    var stateValidator = validationModel[traitName].StateValidator;
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
                    if (command.Value != null && command.Value.Any() && validationModel[traitName].CommandValidators.ContainsKey(command.Key))
                    {
                        var commandValidator = validationModel[traitName].CommandValidators[command.Key];

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

        /// <summary>
        /// Changes the leaf node type to string.
        /// </summary>
        /// <param name="schema">Schema to modify.</param>
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

            foreach (var property in schema.OneOf)
            {
                switch (property.Type)
                {
                    case NJsonSchema.JsonObjectType.String:
                        // Do nothing
                        break;
                    case NJsonSchema.JsonObjectType.Array:
                    case NJsonSchema.JsonObjectType.Object:
                        ChangeLeafNodesToString(property);
                        break;
                    default:
                        property.Type = NJsonSchema.JsonObjectType.String;
                        break;
                }
            }

            foreach (var anyOf in schema.AnyOf)
            {
                foreach (var property in anyOf.Properties)
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

        }

        /// <summary>
        /// Gets device state as a Google device state object in a flattened state.
        /// </summary>
        /// <remarks>
        /// Largely derived from <see cref="Device"/> implementations.
        /// </remarks>
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
    }
}
