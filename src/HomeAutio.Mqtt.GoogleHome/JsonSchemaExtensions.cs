using System.Text.RegularExpressions;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// JSON schema extensions.
    /// </summary>
    public static class JsonSchemaExtensions
    {
        /// <summary>
        /// Validates if the specified path is valid for this schema.
        /// </summary>
        /// <param name="schema">JSON Schema.</param>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns><c>true</c> if path is valid for schema, otherwise <c>false</c>.</returns>
        public static bool ValidateFlattenedPath(this NJsonSchema.JsonSchema schema, string flattenedPath)
        {
            const string delimiter = ".";
            var paths = flattenedPath != null ? flattenedPath.Split(delimiter, 2) : null;
            var currentPathFragment = paths != null ? paths[0] : null;
            var remainingPathFragment = paths != null && paths.Length == 2 ? paths[1] : null;

            switch (schema.Type)
            {
                case NJsonSchema.JsonObjectType.Array:
                    if (currentPathFragment == null)
                        return false;

                    if (schema.Item != null)
                    {
                        if (Regex.IsMatch(currentPathFragment, @"^\[\d+\]$"))
                        {
                            // Unwrap array item
                            if (ValidateFlattenedPath(schema.Item, remainingPathFragment))
                            {
                                return true;
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
                                if (ValidateFlattenedPath(branch, remainingPathFragment))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    break;
                case NJsonSchema.JsonObjectType.Object:
                    if (currentPathFragment == null)
                        return false;

                    if (schema.Properties != null && schema.Properties.ContainsKey(currentPathFragment))
                    {
                        // Recursive traversal of objects
                        if (ValidateFlattenedPath(schema.Properties[currentPathFragment], remainingPathFragment))
                        {
                            return true;
                        }
                    }

                    if (schema.AnyOf != null)
                    {
                        foreach (var propertySchema in schema.AnyOf)
                        {
                            // Unwrap and validate each as a possibility
                            if (ValidateFlattenedPath(propertySchema, flattenedPath))
                            {
                                return true;
                            }
                        }
                    }

                    if (schema.OneOf != null)
                    {
                        foreach (var propertySchema in schema.OneOf)
                        {
                            // Unwrap and validate each as a possibility
                            if (ValidateFlattenedPath(propertySchema, flattenedPath))
                            {
                                return true;
                            }
                        }
                    }

                    break;
                case NJsonSchema.JsonObjectType.None:
                case NJsonSchema.JsonObjectType.Integer:
                case NJsonSchema.JsonObjectType.Number:
                case NJsonSchema.JsonObjectType.Boolean:
                case NJsonSchema.JsonObjectType.String:
                    // Matching leaf node found
                    return true;
            }

            return false;
        }
    }
}
