using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models;
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

            var traitSchemas = TraitSchemaProvider.GetTraitSchemas().GetAwaiter().GetResult();
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
                    // TODO: Transform schema validation instead of checking output?
                    var stateJson = JsonConvert.SerializeObject(GetFakeGoogleState(deviceTrait.State));
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
                    if (command.Value != null && command.Value.Any() && traitSchema.CommandSchemas.Any(x => x.Command == commandType))
                    {
                        var commandValidator = traitSchema.CommandSchemas.First(x => x.Command == commandType).Validator;

                        // Modify the schema validation, only looking for presence not type, etc. matching
                        ChangeLeafNodesToString(commandValidator);

                        // TODO: Transform schema validation instead of checking output?
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
            // Normal properties
            foreach (var property in schema.Properties)
            {
                switch (property.Value.Type)
                {
                    case NJsonSchema.JsonObjectType.Array:
                        foreach (var branch in property.Value.Items)
                        {
                            ChangeLeafNodesToString(branch);
                        }

                        break;
                    case NJsonSchema.JsonObjectType.Object:
                        ChangeLeafNodesToString(property.Value);
                        break;
                    case NJsonSchema.JsonObjectType.Integer:
                    case NJsonSchema.JsonObjectType.Number:
                    case NJsonSchema.JsonObjectType.Boolean:
                        property.Value.Type = NJsonSchema.JsonObjectType.String;
                        break;
                    case NJsonSchema.JsonObjectType.String:
                    default:
                        // Do nothing
                        break;
                }
            }

            // At-most-one properties
            foreach (var property in schema.OneOf)
            {
                switch (property.Type)
                {
                    case NJsonSchema.JsonObjectType.Array:
                        foreach (var branch in property.Items)
                        {
                            ChangeLeafNodesToString(branch);
                        }

                        break;
                    case NJsonSchema.JsonObjectType.Object:
                        ChangeLeafNodesToString(property);
                        break;
                    case NJsonSchema.JsonObjectType.Integer:
                    case NJsonSchema.JsonObjectType.Number:
                    case NJsonSchema.JsonObjectType.Boolean:
                        property.Type = NJsonSchema.JsonObjectType.String;
                        break;
                    case NJsonSchema.JsonObjectType.String:
                    default:
                        // Do nothing
                        break;
                }
            }

            // At-least-one properties
            foreach (var anyOf in schema.AnyOf)
            {
                foreach (var property in anyOf.Properties)
                {
                    switch (property.Value.Type)
                    {
                        case NJsonSchema.JsonObjectType.Array:
                            foreach (var branch in property.Value.Items)
                            {
                                ChangeLeafNodesToString(branch);
                            }

                            break;
                        case NJsonSchema.JsonObjectType.Object:
                            ChangeLeafNodesToString(property.Value);
                            break;
                        case NJsonSchema.JsonObjectType.Integer:
                        case NJsonSchema.JsonObjectType.Number:
                        case NJsonSchema.JsonObjectType.Boolean:
                            property.Value.Type = NJsonSchema.JsonObjectType.String;
                            break;
                        case NJsonSchema.JsonObjectType.String:
                        default:
                            // Do nothing
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets device state as a Google device state object in a flattened state with dummy data for initial validation.
        /// </summary>
        /// <param name="stateConfigs">Current state cache.</param>
        /// <returns>A Google device state object in a flattened state.</returns>
        private static IDictionary<string, object> GetFakeGoogleState(IDictionary<string, DeviceState> stateConfigs)
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

            return stateValues.ToNestedDictionary();
        }
    }
}
