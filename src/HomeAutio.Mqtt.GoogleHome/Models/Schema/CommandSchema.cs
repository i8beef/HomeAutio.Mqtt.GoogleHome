using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// Validator instance.
        /// </summary>
        public JsonSchema Validator { get; private set; }

        /// <summary>
        /// Instantiates from supplied JSON string.
        /// </summary>
        /// <param name="paramJson">Param JSON to parse.</param>
        /// <param name="resultsJson">Results JSON to parse.</param>
        /// <param name="errorJson">Error JSON to parse.</param>
        /// <returns>An instantiated <see cref="CommandSchema"/>.</returns>
        public static async Task<CommandSchema> FromJson(string paramJson, string resultsJson, string errorJson)
        {
            if (string.IsNullOrEmpty(paramJson))
                return null;

            var commandSchema = new CommandSchema
            {
                ErrorJson = errorJson,
                Examples = ExtractExamples(paramJson),
                ParamJson = paramJson,
                ResultsJson = resultsJson,
                Validator = await JsonSchema.FromJsonAsync(paramJson)
            };

            return commandSchema;
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
