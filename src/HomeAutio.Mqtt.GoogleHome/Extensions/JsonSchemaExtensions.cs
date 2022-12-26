using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome.Extensions
{
    /// <summary>
    /// JSON schema extensions.
    /// </summary>
    public static partial class JsonSchemaExtensions
    {
        /// <summary>
        /// Gets the Google type for the specified path is valid for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>The <see cref="GoogleType"/> for the specified path.</returns>
        public static ICollection<object>? GetEnumValuesForFlattenedPath(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            var foundSchemas = schema.GetByFlattenedPath(flattenedPath);

            var result = new List<object>();
            foreach (var foundSchema in foundSchemas)
            {
                if (foundSchema.IsEnumeration)
                {
                    result.AddRange(foundSchema.Enumeration);
                }
            }

            return result.Any() ? result : null;
        }

        /// <summary>
        /// Gets the Google type for the specified path is valid for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>The <see cref="GoogleType"/> for the specified path.</returns>
        public static GoogleType GetGoogleTypeForFlattenedPath(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            var foundSchemas = schema.GetByFlattenedPath(flattenedPath);
            if (!foundSchemas.Any())
            {
                return GoogleType.Unknown;
            }

            foreach (var foundSchema in foundSchemas)
            {
                switch (foundSchema.Type)
                {
                    case NJsonSchema.JsonObjectType.Integer:
                    case NJsonSchema.JsonObjectType.Number:
                        return GoogleType.Numeric;
                    case NJsonSchema.JsonObjectType.Boolean:
                        return GoogleType.Bool;
                    case NJsonSchema.JsonObjectType.String:
                        return GoogleType.String;
                }
            }

            return GoogleType.Unknown;
        }

        /// <summary>
        /// Validates if the specified path is valid for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns><c>true</c> if path is valid for schema, otherwise <c>false</c>.</returns>
        public static bool FlattenedPathExists(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            return schema.GetByFlattenedPath(flattenedPath).Any();
        }

        /// <summary>
        /// Gets the schema item for the specified path for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>A list of matching <see cref="NJsonSchema.JsonSchema"/> for the specified path.</returns>
        public static IList<NJsonSchema.JsonSchema> GetByFlattenedPath(this NJsonSchema.JsonSchema schema, string? flattenedPath)
        {
            const string delimiter = ".";
            var paths = flattenedPath?.Split(delimiter, 2);
            var currentPathFragment = paths?[0];
            var remainingPathFragment = paths != null && paths.Length == 2 ? paths[1] : null;

            switch (schema.Type)
            {
                case NJsonSchema.JsonObjectType.Object:
                case NJsonSchema.JsonObjectType.None:
                    // Treat unspecified types as possible subschemas
                    if (currentPathFragment == null)
                    {
                        return new List<NJsonSchema.JsonSchema>();
                    }

                    var objectResults = new List<NJsonSchema.JsonSchema>();
                    if (schema.Properties != null && schema.Properties.TryGetValue(currentPathFragment, out var foundPropertySchema))
                    {
                        // Recursive traversal of objects
                        objectResults.AddRange(foundPropertySchema.GetByFlattenedPath(remainingPathFragment));
                    }

                    if (schema.AnyOf != null)
                    {
                        foreach (var propertySchema in schema.AnyOf)
                        {
                            // Unwrap and validate each as a possibility
                            objectResults.AddRange(propertySchema.GetByFlattenedPath(flattenedPath));
                        }
                    }

                    if (schema.OneOf != null)
                    {
                        foreach (var propertySchema in schema.OneOf)
                        {
                            // Unwrap and validate each as a possibility
                            objectResults.AddRange(propertySchema.GetByFlattenedPath(flattenedPath));
                        }
                    }

                    if (schema.AdditionalPropertiesSchema != null)
                    {
                        // Use the additional property schema, if available
                        objectResults.AddRange(schema.AdditionalPropertiesSchema.GetByFlattenedPath(flattenedPath));
                    }

                    if (schema.ExtensionData != null && schema.ExtensionData.Any(x => x.Key == "if"))
                    {
                        // Use extension data if present
                        var conditionalKeys = new List<string> { "then", "else" };
                        foreach (var conditionalPropertySchema in schema.ExtensionData.Where(x => conditionalKeys.Contains(x.Key)))
                        {
                            if (conditionalPropertySchema.Value is NJsonSchema.JsonSchema jsonSchema)
                            {
                                // Unwrap and validate each as a possibility
                                objectResults.AddRange(jsonSchema.GetByFlattenedPath(flattenedPath));
                            }
                        }
                    }

                    if (objectResults.Any())
                    {
                        return objectResults;
                    }

                    break;
                case NJsonSchema.JsonObjectType.Array:
                    if (currentPathFragment == null)
                    {
                        return new List<NJsonSchema.JsonSchema>();
                    }

                    if (schema.Item != null)
                    {
                        if (ArrayRegex().IsMatch(currentPathFragment))
                        {
                            // Unwrap array item
                            return schema.Item.GetByFlattenedPath(remainingPathFragment);
                        }
                    }
                    else
                    {
                        var arrayItemResult = new List<NJsonSchema.JsonSchema>();
                        foreach (var branch in schema.Items)
                        {
                            if (ArrayRegex().IsMatch(currentPathFragment))
                            {
                                // Unwrap array item
                                arrayItemResult.AddRange(branch.GetByFlattenedPath(remainingPathFragment));
                            }
                        }

                        if (arrayItemResult.Any())
                        {
                            return arrayItemResult;
                        }
                    }

                    break;
                case NJsonSchema.JsonObjectType.Integer:
                case NJsonSchema.JsonObjectType.Number:
                case NJsonSchema.JsonObjectType.Boolean:
                case NJsonSchema.JsonObjectType.String:
                    // Matching leaf node found
                    return new List<NJsonSchema.JsonSchema> { schema };
            }

            return new List<NJsonSchema.JsonSchema>();
        }

        [GeneratedRegex(@"^\[\d+\]$")]
        private static partial Regex ArrayRegex();
    }
}
