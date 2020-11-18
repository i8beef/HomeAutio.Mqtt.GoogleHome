using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// Schema validation provider.
    /// </summary>
    /// <remarks>
    /// Warning: Here be dragons.
    /// </remarks>
    public static class SchemaValidationProvider
    {
        private static IDictionary<string, TraitValidationDefinition> _traitSchemaCache = null;

        /// <summary>
        /// Get trait schemas for validation.
        /// </summary>
        /// <returns>A dictionary of trait schemas.</returns>
        public static async Task<IDictionary<string, TraitValidationDefinition>> GetTraitSchemas()
        {
            if (_traitSchemaCache == null)
            {
                var traitTypesNames = Enum.GetNames(typeof(TraitType));
                var traitTypes = traitTypesNames
                    .Where(x => x.ToLower() != "unknown")
                    .Select(x => Enum.Parse<TraitType>(x).ToEnumString());

                var commandNames = Enum.GetNames(typeof(CommandType));
                var commands = commandNames
                    .Where(x => x.ToLower() != "unknown")
                    .Select(x => Enum.Parse<CommandType>(x).ToEnumString());

                var assembly = typeof(TraitType).Assembly;
                var resources = assembly.GetManifestResourceNames();

                var allTraitsResourceBase = "HomeAutio.Mqtt.GoogleHome.schema.traits";

                var result = new Dictionary<string, TraitValidationDefinition>();
                foreach (var trait in traitTypes)
                {
                    var traitName = trait.Substring(trait.LastIndexOf('.') + 1).ToLower();
                    var traitResourceBase = $"{allTraitsResourceBase}.{traitName}";
                    if (resources.Any(x => x.StartsWith(traitResourceBase)))
                    {
                        // Attributes
                        var attributeFile = $"{traitResourceBase}.{traitName}.attributes.schema.json";
                        string attributeSchema = null;
                        var attributeExamples = new List<TraitExample>();
                        if (resources.Contains(attributeFile))
                        {
                            attributeSchema = GetResourceString(attributeFile);
                            var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(attributeSchema, new ObjectDictionaryConverter());

                            // Attribute example
                            if (parsed.ContainsKey("examples") && parsed["examples"] is List<object> examples)
                            {
                                // Get distinct examples
                                var seenAttributeExamples = new List<string>();
                                foreach (Dictionary<string, object> example in examples)
                                {
                                    // Strip comments
                                    var exampleWithoutComment = example
                                        .Where(x => x.Key != "$comment")
                                        .ToDictionary(kv => kv.Key, kv => kv.Value);

                                    var traitExample = new TraitExample
                                    {
                                        Comment = example["$comment"] as string,
                                        Example = JsonConvert.SerializeObject(exampleWithoutComment, Formatting.Indented)
                                    };

                                    // Eliminate duplicates
                                    if (!seenAttributeExamples.Contains(traitExample.Example))
                                    {
                                        seenAttributeExamples.Add(traitExample.Example);
                                        attributeExamples.Add(traitExample);
                                    }
                                }
                            }
                        }

                        // Commands
                        var commandSchemas = new Dictionary<string, string>();
                        var commandExamples = new Dictionary<string, IList<TraitExample>>();
                        var commandValidators = new Dictionary<string, NJsonSchema.JsonSchema>();
                        foreach (var commandFile in resources.Where(x => x.StartsWith($"{traitResourceBase}") && x.EndsWith(".params.schema.json")))
                        {
                            var commandName = commandFile
                                .Replace($"{traitResourceBase}.", string.Empty)
                                .Replace(".params.schema.json", string.Empty);
                            var fileContents = GetResourceString(commandFile);

                            var command = commands.FirstOrDefault(x => x.Substring(x.LastIndexOf('.') + 1).ToLower() == commandName);

                            // Note: Temporary
                            if (command != null)
                            {
                                commandSchemas.Add(command, fileContents);
                                commandValidators.Add(command, await NJsonSchema.JsonSchema.FromJsonAsync(fileContents));
                                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContents, new ObjectDictionaryConverter());

                                // Command example
                                if (parsed.ContainsKey("examples") && parsed["examples"] is List<object> examples)
                                {
                                    if (!commandExamples.ContainsKey(command))
                                    {
                                        commandExamples.Add(command, new List<TraitExample>());
                                    }

                                    // Get distinct examples
                                    var seenCommandExamples = new List<string>();
                                    foreach (Dictionary<string, object> example in examples)
                                    {
                                        // Strip comments
                                        var exampleWithoutComment = example
                                            .Where(x => x.Key != "$comment")
                                            .ToDictionary(kv => kv.Key, kv => kv.Value)
                                            .ToFlatDictionary()
                                            .ToDictionary(kv => kv.Key, kv => "MQTT_COMMAND_TOPIC");

                                        var commandExample = new Dictionary<string, object>();
                                        commandExample.Add(command, exampleWithoutComment);

                                        var traitExample = new TraitExample
                                        {
                                            Comment = example["$comment"] as string,
                                            Example = JsonConvert.SerializeObject(commandExample, Formatting.Indented)
                                        };

                                        // Eliminate duplicates
                                        if (!seenCommandExamples.Contains(traitExample.Example))
                                        {
                                            seenCommandExamples.Add(traitExample.Example);
                                            commandExamples[command].Add(traitExample);
                                        }
                                    }
                                }

                            }
                        }

                        // State
                        var statesFile = $"{traitResourceBase}.{traitName}.states.schema.json";
                        string statesSchema = null;
                        NJsonSchema.JsonSchema stateValidator = null;
                        var stateExamples = new List<TraitExample>();
                        if (resources.Contains(statesFile))
                        {
                            statesSchema = GetResourceString(statesFile);
                            stateValidator = await NJsonSchema.JsonSchema.FromJsonAsync(statesSchema);
                            var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(statesSchema, new ObjectDictionaryConverter());

                            // State examples
                            if (parsed.ContainsKey("examples") && parsed["examples"] is List<object> examples)
                            {
                                // Build an index of google types for state paths
                                var googleTypes = GetGoogleTypes(stateValidator)
                                    .GroupBy(x => x.Key)
                                    .Select(x => x.FirstOrDefault())
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                                // Get distinct examples
                                var seenStateExamples = new List<string>();
                                foreach (Dictionary<string, object> example in examples)
                                {
                                    // Strip comments
                                    var exampleWithoutComment = example
                                        .Where(x => x.Key != "$comment")
                                        .ToDictionary(kv => kv.Key, kv => kv.Value)
                                        .ToFlatDictionary()
                                        .ToDictionary(kv => kv.Key, kv => new DeviceState { Topic = "MQTT_STATE_TOPIC", GoogleType = googleTypes.ContainsKey(kv.Key) ? googleTypes[kv.Key] : GoogleType.String });

                                    var traitExample = new TraitExample
                                    {
                                        Comment = example["$comment"] as string,
                                        Example = JsonConvert.SerializeObject(exampleWithoutComment, Formatting.Indented)
                                    };

                                    // Eliminate duplicates
                                    if (!seenStateExamples.Contains(traitExample.Example))
                                    {
                                        seenStateExamples.Add(traitExample.Example);
                                        stateExamples.Add(traitExample);
                                    }
                                }
                            }
                        }

                        var schemas = new TraitValidationDefinition
                        {
                            AttributeSchema = attributeSchema,
                            AttributeValidator = attributeSchema != null ? await NJsonSchema.JsonSchema.FromJsonAsync(attributeSchema) : null,
                            AttributeExamples = attributeExamples,
                            CommandSchemas = commandSchemas,
                            CommandValidators = commandValidators,
                            CommandExamples = commandExamples,
                            StateSchema = statesSchema,
                            StateValidator = stateValidator,
                            StateExamples = stateExamples
                        };

                        result.Add(trait, schemas);
                    }
                }

                _traitSchemaCache = result;
            }

            return _traitSchemaCache;
        }

        /// <summary>
        /// Gets the <see cref="GoogleType"/> for the properties specified in the schema.
        /// </summary>
        /// <param name="schema">The parsed schema.</param>
        /// <param name="path">The current path.</param>
        /// <returns>The <see cref="GoogleType"/>.</returns>
        private static IList<KeyValuePair<string, GoogleType>> GetGoogleTypes(NJsonSchema.JsonSchema schema, string path = "")
        {
            var result = new List<KeyValuePair<string, GoogleType>>();
            if (schema != null)
            {
                switch (schema.Type)
                {
                    case NJsonSchema.JsonObjectType.Object:
                        foreach (var property in schema.Properties)
                        {
                            if (string.IsNullOrEmpty(path))
                            {
                                result.AddRange(GetGoogleTypes(property.Value, property.Key));
                            }
                            else
                            {
                                result.AddRange(GetGoogleTypes(property.Value, $"{path}.{property.Key}"));
                            }
                        }

                        foreach (var property in schema.OneOf)
                        {
                            if (string.IsNullOrEmpty(path))
                            {
                                result.AddRange(GetGoogleTypes(property, path));
                            }
                            else
                            {
                                result.AddRange(GetGoogleTypes(property, path));
                            }
                        }

                        foreach (var anyOf in schema.AnyOf)
                        {
                            foreach (var property in anyOf.Properties)
                            {
                                if (string.IsNullOrEmpty(path))
                                {
                                    result.AddRange(GetGoogleTypes(property.Value, property.Key));
                                }
                                else
                                {
                                    result.AddRange(GetGoogleTypes(property.Value, $"{path}.{property.Key}"));
                                }
                            }
                        }

                        break;
                    case NJsonSchema.JsonObjectType.Array:
                        var index = 0;
                        foreach (var item in schema.Items)
                        {
                            result.AddRange(GetGoogleTypes(item, $"{path}[{index}]"));
                            index++;
                        }

                        break;
                    case NJsonSchema.JsonObjectType.Boolean:
                        result.Add(new KeyValuePair<string, GoogleType>(path, GoogleType.Bool));
                        break;
                    case NJsonSchema.JsonObjectType.Integer:
                    case NJsonSchema.JsonObjectType.Number:
                        result.Add(new KeyValuePair<string, GoogleType>(path, GoogleType.Numeric));
                        break;
                    case NJsonSchema.JsonObjectType.String:
                        result.Add(new KeyValuePair<string, GoogleType>(path, GoogleType.String));
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets resource file contents as string.
        /// </summary>
        /// <param name="resourceName">Resource name to retrieve.</param>
        /// <returns>Resource file contents.</returns>
        private static string GetResourceString(string resourceName)
        {
            var assembly = typeof(TraitType).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
