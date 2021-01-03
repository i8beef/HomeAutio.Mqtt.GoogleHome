using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using Newtonsoft.Json;
using NJsonSchema;

namespace HomeAutio.Mqtt.GoogleHome.Models.Schema
{
    /// <summary>
    /// Command schema.
    /// </summary>
    public class CommandSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSchema"/> class.
        /// </summary>
        private CommandSchema() { }

        /// <summary>
        /// Command type.
        /// </summary>
        public CommandType Command { get; private set; }

        /// <summary>
        /// Error JSON.
        /// </summary>
        public string ErrorJson { get; private set; }

        /// <summary>
        /// Examples.
        /// </summary>
        public IList<SchemaExample> Examples { get; private set; }

        /// <summary>
        /// Param JSON.
        /// </summary>
        public string ParamJson { get; private set; }

        /// <summary>
        /// Result JSON.
        /// </summary>
        public string ResultsJson { get; private set; }

        /// <summary>
        /// Results validator instance.
        /// </summary>
        public JsonSchema ResultsValidator { get; private set; }

        /// <summary>
        /// Validator instance.
        /// </summary>
        public JsonSchema Validator { get; private set; }

        /// <summary>
        /// Instantiates from supplied JSON string.
        /// </summary>
        /// <param name="commandType">Command type.</param>
        /// <param name="paramJson">Param JSON to parse.</param>
        /// <param name="resultsJson">Results JSON to parse.</param>
        /// <param name="errorJson">Error JSON to parse.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An instantiated <see cref="CommandSchema"/>.</returns>
        public static async Task<CommandSchema> FromJson(
            CommandType commandType,
            string paramJson,
            string resultsJson,
            string errorJson,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(paramJson))
                return null;

            var resultValidator = resultsJson != null ? await JsonSchema.FromJsonAsync(resultsJson, cancellationToken) : null;
            var validator = await JsonSchema.FromJsonAsync(paramJson, cancellationToken);

            if (resultValidator != null)
            {
                // Convert to be used for validation
                ChangeOneOfsToAnyOfs(resultValidator);
            }

            if (validator != null)
            {
                // Convert to be used for validation
                ChangeOneOfsToAnyOfs(validator);

                // Modify the schema validation, only looking for presence not type or value
                ChangeLeafNodesToString(validator);
            }

            var commandSchema = new CommandSchema
            {
                Command = commandType,
                ErrorJson = errorJson,
                Examples = ExtractExamples(paramJson),
                ParamJson = paramJson,
                ResultsJson = resultsJson,
                ResultsValidator = resultValidator,
                Validator = validator
            };

            return commandSchema;
        }

        /// <summary>
        /// Change OneOfs to AnyOfs.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        private static void ChangeOneOfsToAnyOfs(JsonSchema schema)
        {
            if (schema.OneOf != null && schema.OneOf.Any())
            {
                foreach (var oneOf in schema.OneOf)
                {
                    oneOf.AllowAdditionalProperties = true;
                    schema.AnyOf.Add(oneOf);
                }

                schema.OneOf.Clear();
            }

            // Recursive apply
            switch (schema.Type)
            {
                case JsonObjectType.Object:
                case JsonObjectType.None:
                    // Treat unspecified types as possible subschemas
                    if (schema.Properties != null)
                    {
                        foreach (var property in schema.Properties)
                        {
                            ChangeOneOfsToAnyOfs(property.Value);
                        }
                    }

                    if (schema.AnyOf != null)
                    {
                        foreach (var propertySchema in schema.AnyOf)
                        {
                            ChangeOneOfsToAnyOfs(propertySchema);
                        }
                    }

                    if (schema.OneOf != null)
                    {
                        foreach (var propertySchema in schema.OneOf)
                        {
                            ChangeOneOfsToAnyOfs(propertySchema);
                        }
                    }

                    if (schema.AdditionalPropertiesSchema != null)
                    {
                        ChangeOneOfsToAnyOfs(schema.AdditionalPropertiesSchema);
                    }

                    break;
                case JsonObjectType.Array:
                    if (schema.Item != null)
                    {
                        ChangeOneOfsToAnyOfs(schema.Item);
                    }
                    else
                    {
                        foreach (var branch in schema.Items)
                        {
                            ChangeOneOfsToAnyOfs(branch);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Changes the leaf node type to string.
        /// </summary>
        /// <param name="schema">Schema to modify.</param>
        private static void ChangeLeafNodesToString(JsonSchema schema)
        {
            switch (schema.Type)
            {
                case JsonObjectType.Object:
                case JsonObjectType.None:
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

                    if (schema.AdditionalPropertiesSchema != null)
                    {
                        ChangeLeafNodesToString(schema.AdditionalPropertiesSchema);
                    }

                    break;
                case JsonObjectType.Array:
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
                case JsonObjectType.Integer:
                case JsonObjectType.Number:
                case JsonObjectType.Boolean:
                case JsonObjectType.String:
                default:
                    // Replace with simple schema
                    schema.Type = JsonObjectType.None;
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
        /// Extract example JSON from a schema.
        /// </summary>
        /// <param name="json">Schema JSON to parse.</param>
        /// <returns>A list of <see cref="SchemaExample"/>.</returns>
        private static IList<SchemaExample> ExtractExamples(string json)
        {
            // Attribute example
            var examples = new List<SchemaExample>();
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, new ObjectDictionaryConverter());
            if (parsed.ContainsKey("examples") && parsed["examples"] is List<object> exampleNodes)
            {
                // Get distinct examples
                var seenAttributeExamples = new List<string>();
                foreach (Dictionary<string, object> example in exampleNodes)
                {
                    // Strip comments
                    var exampleWithoutComment = example
                        .Where(x => x.Key != "$comment")
                        .ToDictionary(kv => kv.Key, kv => kv.Value)
                        .ToFlatDictionary()
                        .ToDictionary(kv => kv.Key, kv => "MQTT_COMMAND_TOPIC");

                    var traitExample = new SchemaExample
                    {
                        Comment = example["$comment"] as string,
                        Example = JsonConvert.SerializeObject(exampleWithoutComment, Formatting.Indented)
                    };

                    // Eliminate duplicates
                    if (!seenAttributeExamples.Contains(traitExample.Example))
                    {
                        seenAttributeExamples.Add(traitExample.Example);
                        examples.Add(traitExample);
                    }
                }
            }

            return examples;
        }
    }
}
