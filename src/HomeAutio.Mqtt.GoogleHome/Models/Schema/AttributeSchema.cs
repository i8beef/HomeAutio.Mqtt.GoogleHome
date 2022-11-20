using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NJsonSchema;

namespace HomeAutio.Mqtt.GoogleHome.Models.Schema
{
    /// <summary>
    /// Attribute schema.
    /// </summary>
    public class AttributeSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeSchema"/> class.
        /// </summary>
        private AttributeSchema() { }

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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An instantiated <see cref="AttributeSchema"/>.</returns>
        public static async Task<AttributeSchema> FromJson(string json, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var attributeSchema = new AttributeSchema
            {
                Examples = ExtractExamples(json),
                Json = json,
                Validator = await JsonSchema.FromJsonAsync(json, cancellationToken)
            };

            return attributeSchema;
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
            if (parsed.TryGetValue("examples", out var parsedExamples) && parsedExamples is List<object> exampleNodes)
            {
                // Get distinct examples
                var seenAttributeExamples = new List<string>();
                foreach (var example in exampleNodes.Cast<Dictionary<string, object>>())
                {
                    // Strip comments
                    var exampleWithoutComment = example
                        .Where(x => x.Key != "$comment")
                        .ToDictionary(kv => kv.Key, kv => kv.Value);

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
