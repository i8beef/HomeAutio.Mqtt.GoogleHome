﻿using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models;
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

                    validationErrors.AddRange(attributeErrors.Select(x => $"{x.Path}: {x.Kind}"));
                }

                // State validation
                if (deviceTrait.State != null && traitSchema.StateSchema?.Validator != null)
                {
                    var stateJson = JsonConvert.SerializeObject(GetFakeGoogleState(deviceTrait.State, traitSchema));
                    var stateErrors = traitSchema.StateSchema.Validator.Validate(stateJson);

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
                    var commandType = command.Key.ToEnum<CommandType>();
                    if (command.Value != null && traitSchema.CommandSchemas.Any(x => x.Command == commandType))
                    {
                        var commandValidator = traitSchema.CommandSchemas.First(x => x.Command == commandType).Validator;

                        // Modify the schema validation, only looking for presence not type or value
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
            switch (schema.Type)
            {
                case NJsonSchema.JsonObjectType.Array:
                    if (schema.Item != null)
                    {
                        // Default single type array
                        ChangeLeafNodesToString(schema.Item);
                    }
                    else
                    {
                        // Tuple handling
                        foreach (var tupleSchema in schema.Items)
                        {
                            ChangeLeafNodesToString(tupleSchema);
                        }
                    }

                    break;
                case NJsonSchema.JsonObjectType.Object:
                    foreach (var property in schema.Properties)
                    {
                        ChangeLeafNodesToString(property.Value);
                    }

                    foreach (var property in schema.OneOf)
                    {
                        ChangeLeafNodesToString(property);
                    }

                    foreach (var property in schema.AnyOf)
                    {
                        ChangeLeafNodesToString(property);
                    }

                    if (schema.AllowAdditionalProperties)
                    {
                        foreach (var property in schema.AdditionalPropertiesSchema.Properties)
                        {
                            ChangeLeafNodesToString(property.Value);
                        }
                    }

                    break;
                case NJsonSchema.JsonObjectType.None:
                case NJsonSchema.JsonObjectType.Integer:
                case NJsonSchema.JsonObjectType.Number:
                case NJsonSchema.JsonObjectType.Boolean:
                case NJsonSchema.JsonObjectType.String:
                default:
                    // Replace with simple schema
                    schema.Type = NJsonSchema.JsonObjectType.None;
                    if (schema.IsEnumeration)
                    {
                        schema.Enumeration.Clear();
                    }

                    if (!string.IsNullOrEmpty(schema.Pattern))
                    {
                        schema.Pattern = null;
                    }

                    break;
            }
        }

        /// <summary>
        /// Gets device state as a Google device state object in a flattened state with dummy data for initial validation.
        /// </summary>
        /// <param name="stateConfigs">Current state cache.</param>
        /// <param name="traitSchema">Trait schema.</param>
        /// <returns>A Google device state object in a flattened state.</returns>
        private static IDictionary<string, object> GetFakeGoogleState(IDictionary<string, DeviceState> stateConfigs, TraitSchema traitSchema)
        {
            var stateValues = new Dictionary<string, object>();
            foreach (var state in stateConfigs)
            {
                if (state.Value.Topic != null)
                {
                    var googleType = traitSchema.GetGoogleTypeForFlattenedPath(state.Key);
                    switch (googleType)
                    {
                        case GoogleType.Bool:
                            stateValues.Add(state.Key, state.Value.MapValueToGoogle("true", googleType));
                            break;
                        case GoogleType.Numeric:
                            stateValues.Add(state.Key, state.Value.MapValueToGoogle("1", googleType));
                            break;
                        case GoogleType.String:
                        default:
                            stateValues.Add(state.Key, state.Value.MapValueToGoogle("default", googleType));
                            break;
                    }
                }
            }

            return stateValues.ToNestedDictionary();
        }
    }
}
