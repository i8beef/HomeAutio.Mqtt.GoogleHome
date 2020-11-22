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
                        ChangeLeafNodesToString(schema.Item);
                    }
                    else
                    {
                        foreach (var branch in schema.Items)
                        {
                            ChangeLeafNodesToString(branch);
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

                    break;
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
