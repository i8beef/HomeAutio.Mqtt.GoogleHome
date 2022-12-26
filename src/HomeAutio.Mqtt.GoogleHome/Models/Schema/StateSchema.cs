using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
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
        /// JSON.
        /// </summary>
        public required string Json { get; init; }

        /// <summary>
        /// Examples.
        /// </summary>
        public required IList<SchemaExample> Examples { get; init; }

        /// <summary>
        /// Validator instance.
        /// </summary>
        public required JsonSchema Validator { get; init; }

        /// <summary>
        /// Instantiates from supplied JSON string.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An instantiated <see cref="StateSchema"/>.</returns>
        public static async Task<StateSchema?> FromJson(string? json, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var stateValidator = await JsonSchema.FromJsonAsync(json, cancellationToken);
            var stateSchema = new StateSchema
            {
                Examples = ExtractExamples(json),
                Json = json,
                Validator = stateValidator
            };

            return stateSchema;
        }

        /// <summary>
        /// Extract example JSON from a schema.
        /// </summary>
        /// <param name="json">Schema JSON to parse.</param>
        /// <returns>A list of <see cref="SchemaExample"/>.</returns>
        private static IList<SchemaExample> ExtractExamples(string json)
        {
            var examples = new List<SchemaExample>();
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, new ObjectDictionaryConverter());
            if (parsed is not null && parsed.TryGetValue("examples", out var parsedExamples) && parsedExamples is List<object> exampleNodes)
            {
                // Get distinct examples
                var seenAttributeExamples = new List<string>();
                foreach (var example in exampleNodes.Cast<Dictionary<string, object?>>())
                {
                    // Strip comments
                    var exampleWithoutComment = example
                        .Where(x => x.Key != "$comment")
                        .ToDictionary(kv => kv.Key, kv => kv.Value)
                        .ToFlatDictionary()
                        .ToDictionary(kv => kv.Key, kv => new DeviceState { Topic = "MQTT_STATE_TOPIC" });

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
