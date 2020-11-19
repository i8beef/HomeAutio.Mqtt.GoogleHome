using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Newtonsoft.Json;
using NJsonSchema;

namespace HomeAutio.Mqtt.GoogleHome.Models.Schema
{
    /// <summary>
    /// State schema.
    /// </summary>
    public class StateSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateSchema"/> class.
        /// </summary>
        private StateSchema() { }

        /// <summary>
        /// JSON.
        /// </summary>
        public string Json { get; private set; }

        /// <summary>
        /// Examples.
        /// </summary>
        public IList<SchemaExample> Examples { get; private set; }

        /// <summary>
        /// Validator instance.
        /// </summary>
        public JsonSchema Validator { get; private set; }

        /// <summary>
        /// Instantiates from supplied JSON string.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>An instantiated <see cref="StateSchema"/>.</returns>
        public static async Task<StateSchema> FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var stateValidator = await JsonSchema.FromJsonAsync(json);

            // Build an index of google types for state paths
            var googleTypes = GetGoogleTypes(stateValidator)
                .GroupBy(x => x.Key)
                .Select(x => x.FirstOrDefault())
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var stateSchema = new StateSchema
            {
                Examples = ExtractExamples(json, googleTypes),
                Json = json,
                Validator = stateValidator
            };

            return stateSchema;
        }

        /// <summary>
        /// Extract example JSON from a schema.
        /// </summary>
        /// <param name="json">Schema JSON to parse.</param>
        /// <param name="googleTypes">Google type mappings for properties.</param>
        /// <returns>A list of <see cref="SchemaExample"/>.</returns>
        private static IList<SchemaExample> ExtractExamples(string json, IDictionary<string, GoogleType> googleTypes)
        {
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
                        .ToDictionary(kv => kv.Key, kv => new DeviceState { Topic = "MQTT_STATE_TOPIC", GoogleType = googleTypes.ContainsKey(kv.Key) ? googleTypes[kv.Key] : GoogleType.String });

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

        /// <summary>
        /// Gets the <see cref="GoogleType"/> for the properties specified in the schema.
        /// </summary>
        /// <param name="schema">The parsed schema.</param>
        /// <param name="path">The current path.</param>
        /// <returns>The <see cref="GoogleType"/>.</returns>
        private static IList<KeyValuePair<string, GoogleType>> GetGoogleTypes(JsonSchema schema, string path = "")
        {
            var result = new List<KeyValuePair<string, GoogleType>>();
            if (schema != null)
            {
                switch (schema.Type)
                {
                    case JsonObjectType.Object:
                        foreach (var property in schema.Properties)
                        {
                            if (string.IsNullOrEmpty(path))
                                result.AddRange(GetGoogleTypes(property.Value, property.Key));
                            else
                                result.AddRange(GetGoogleTypes(property.Value, $"{path}.{property.Key}"));
                        }

                        foreach (var property in schema.OneOf)
                        {
                            if (string.IsNullOrEmpty(path))
                                result.AddRange(GetGoogleTypes(property, path));
                            else
                                result.AddRange(GetGoogleTypes(property, path));
                        }

                        foreach (var anyOf in schema.AnyOf)
                        {
                            foreach (var property in anyOf.Properties)
                            {
                                if (string.IsNullOrEmpty(path))
                                    result.AddRange(GetGoogleTypes(property.Value, property.Key));
                                else
                                    result.AddRange(GetGoogleTypes(property.Value, $"{path}.{property.Key}"));
                            }
                        }

                        break;
                    case JsonObjectType.Array:
                        var index = 0;
                        foreach (var item in schema.Items)
                        {
                            result.AddRange(GetGoogleTypes(item, $"{path}[{index}]"));
                            index++;
                        }

                        break;
                    case JsonObjectType.Boolean:
                        result.Add(new KeyValuePair<string, GoogleType>(path, GoogleType.Bool));
                        break;
                    case JsonObjectType.Integer:
                    case JsonObjectType.Number:
                        result.Add(new KeyValuePair<string, GoogleType>(path, GoogleType.Numeric));
                        break;
                    case JsonObjectType.String:
                        result.Add(new KeyValuePair<string, GoogleType>(path, GoogleType.String));
                        break;
                }
            }

            return result;
        }
    }
}
