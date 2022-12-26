using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models.Schema;
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

            var traitSchemas = TraitSchemaProvider.GetTraitSchemas();
            var traitSchema = traitSchemas.FirstOrDefault(x => x.Trait == deviceTrait.Trait);
            if (traitSchema != null)
            {
                // Attribute validation
                if (deviceTrait.Attributes != null && traitSchema.AttributeSchema?.Validator != null)
                {
                    var attributeJson = JsonConvert.SerializeObject(deviceTrait.Attributes);
                    var attributeErrors = traitSchema.AttributeSchema.Validator.Validate(attributeJson);

                    validationErrors.AddRange(attributeErrors.Select(x => $"Attributes: {x.Path}: {x.Kind}"));
                }

                //// State validation
                ////if (deviceTrait.State != null && traitSchema.StateSchema?.Validator != null)
                ////{
                ////    var stateJson = JsonConvert.SerializeObject(GetFakeGoogleState(deviceTrait.State, traitSchema));
                ////    var stateErrors = traitSchema.StateSchema.Validator.Validate(stateJson);

                ////    validationErrors.AddRange(stateErrors.Select(x => $"State: {x.Path}: {x.Kind}"));
                ////}

                //// Command validations
                ////var deviceCommands = deviceTrait.Commands.ToDictionary(
                ////    k => k.Key,
                ////    v => v.Value?.ToDictionary(
                ////        x => x.Key,
                ////        x => (object)x.Value).ToNestedDictionary());

                ////foreach (var command in deviceCommands)
                ////{
                ////    var commandType = command.Key.ToEnum<CommandType>();
                ////    if (command.Value != null && traitSchema.CommandSchemas.Any(x => x.Command == commandType))
                ////    {
                ////        var commandValidator = traitSchema.CommandSchemas.First(x => x.Command == commandType).Validator;
                ////        var commandJson = JsonConvert.SerializeObject(command.Value);
                ////        var commandErrors = commandValidator.Validate(commandJson);

                ////        validationErrors.AddRange(commandErrors.Select(x => $"Commands ({command.Key}): {x.Path}: {x.Kind}"));
                ////    }
                ////}
            }

            return validationErrors;
        }

#pragma warning disable IDE0051 // Remove unused private members

        /// <summary>
        /// Gets device state as a Google device state object in a flattened state with dummy data for initial validation.
        /// </summary>
        /// <param name="stateConfigs">Current state cache.</param>
        /// <param name="traitSchema">Trait schema.</param>
        /// <returns>A Google device state object in a flattened state.</returns>
        private static IDictionary<string, object?> GetFakeGoogleState(IDictionary<string, DeviceState> stateConfigs, TraitSchema traitSchema)
        {
            var stateValues = new Dictionary<string, object?>();
            foreach (var state in stateConfigs)
            {
                var googleType = traitSchema.GetGoogleTypeForFlattenedPath(state.Key);
                var enumValues = traitSchema.GetEnumValuesForFlattenedPath(state.Key);
                var enumValue = enumValues?.FirstOrDefault()?.ToString();
                switch (googleType)
                {
                    case GoogleType.Bool:
                        stateValues.Add(state.Key, state.Value.MapValueToGoogle(enumValue ?? "true", googleType));
                        break;
                    case GoogleType.Numeric:
                        stateValues.Add(state.Key, state.Value.MapValueToGoogle(enumValue ?? "1", googleType));
                        break;
                    case GoogleType.String:
                    default:
                        stateValues.Add(state.Key, state.Value.MapValueToGoogle(enumValue ?? "default", googleType));
                        break;
                }
            }

            return stateValues.ToNestedDictionary();
        }
    }
#pragma warning restore IDE0051 // Remove unused private members
}
