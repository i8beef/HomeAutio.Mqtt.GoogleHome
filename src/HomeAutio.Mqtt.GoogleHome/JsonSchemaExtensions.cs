using System.Text.RegularExpressions;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// JSON schema extensions.
    /// </summary>
    public static class JsonSchemaExtensions
    {
        /// <summary>
        /// Gets the Google type for the specified path is valid for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>The <see cref="GoogleType"/> for the specified path.</returns>
        public static GoogleType GetGoogleTypeForFlattenedPath(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            var foundSchema = schema.GetByFlattenedPath(flattenedPath);
            if (foundSchema == null)
            {
                return GoogleType.Unknown;
            }

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

            return GoogleType.Unknown;
        }

        /// <summary>
        /// Validates if the specified path is valid for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns><c>true</c> if path is valid for schema, otherwise <c>false</c>.</returns>
        public static bool ValidateFlattenedPath(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            return schema.GetByFlattenedPath(flattenedPath) != null;
        }

        /// <summary>
        /// Gets the schema item for the specified path for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>The <see cref="NJsonSchema.JsonSchema"/> for the specified path.</returns>
        public static NJsonSchema.JsonSchema GetByFlattenedPath(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            const string delimiter = ".";
            var paths = flattenedPath?.Split(delimiter, 2);
            var currentPathFragment = paths?[0];
            var remainingPathFragment = paths != null && paths.Length == 2 ? paths[1] : null;

            switch (schema.Type)
            {
                case NJsonSchema.JsonObjectType.Object:
                    if (currentPathFragment == null)
                        return null;

                    if (schema.Properties != null && schema.Properties.ContainsKey(currentPathFragment))
                    {
                        // Recursive traversal of objects
                        return GetByFlattenedPath(schema.Properties[currentPathFragment], remainingPathFragment);
                    }

                    if (schema.AnyOf != null)
                    {
                        foreach (var propertySchema in schema.AnyOf)
                        {
                            // Unwrap and validate each as a possibility
                            var anyOfResult = GetByFlattenedPath(propertySchema, flattenedPath);
                            if (anyOfResult != null)
                            {
                                return anyOfResult;
                            }
                        }
                    }

                    if (schema.OneOf != null)
                    {
                        foreach (var propertySchema in schema.OneOf)
                        {
                            // Unwrap and validate each as a possibility
                            var oneOfResult = GetByFlattenedPath(propertySchema, flattenedPath);
                            if (oneOfResult != null)
                            {
                                return oneOfResult;
                            }
                        }
                    }

                    if (schema.AllowAdditionalProperties)
                    {
                        // Use the additional property schema, if available
                        return schema.AdditionalPropertiesSchema;
                    }

                    break;
                case NJsonSchema.JsonObjectType.Array:
                    if (currentPathFragment == null)
                        return null;

                    if (schema.Item != null)
                    {
                        if (Regex.IsMatch(currentPathFragment, @"^\[\d+\]$"))
                        {
                            // Unwrap array item
                            var itemResult = GetByFlattenedPath(schema.Item, remainingPathFragment);
                            if (itemResult != null)
                            {
                                return itemResult;
                            }
                        }
                    }
                    else
                    {
                        foreach (var branch in schema.Items)
                        {
                            if (Regex.IsMatch(currentPathFragment, @"^\[\d+\]$"))
                            {
                                // Unwrap array item
                                var breanchItemResult = GetByFlattenedPath(branch, remainingPathFragment);
                                if (breanchItemResult != null)
                                {
                                    return breanchItemResult;
                                }
                            }
                        }
                    }

                    break;
                case NJsonSchema.JsonObjectType.Integer:
                case NJsonSchema.JsonObjectType.Number:
                case NJsonSchema.JsonObjectType.Boolean:
                case NJsonSchema.JsonObjectType.String:
                    // Matching leaf node found
                    return schema;
            }

            return null;
        }
    }
}
