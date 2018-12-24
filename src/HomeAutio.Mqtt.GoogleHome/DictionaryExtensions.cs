using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Dictionary extensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Flattens a nested dictionary with keys joined by the specified delimiter.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <returns>A flattened Dictionary.</returns>
        public static IDictionary<string, object> ToFlatDictionary(this IDictionary<string, object> source, string delimiter = ".")
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in source)
            {
                if (kvp.Value is IDictionary<string, object>)
                {
                    var flattenedValueDictionary = ((IDictionary<string, object>)kvp.Value).ToFlatDictionary(delimiter);
                    foreach (var subKvp in flattenedValueDictionary)
                    {
                        result.Add(string.Join(delimiter, kvp.Key, subKvp.Key), subKvp.Value);
                    }
                }
                else
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Flattens a nested dictionary with keys joined by the specified delimiter.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <returns>A flattened Dictionary.</returns>
        public static IDictionary<string, object> ToNestedDictionary(this IDictionary<string, object> source, string delimiter = ".")
        {
            var result = new Dictionary<string, object>();

            foreach (var kvp in source)
            {
                if (kvp.Value is IDictionary<string, object>)
                {
                    throw new System.Exception("Cannot convert a non-flat Dictionary to a nested dictionary.");
                }

                if (kvp.Key.Contains(delimiter))
                {
                    // Split and recursively add
                    var parts = new Queue<string>(kvp.Key.Split(delimiter));

                    // Start with the root result object
                    var parent = result;
                    while (parts.Count > 0)
                    {
                        var part = parts.Dequeue();
                        if (parts.Count > 0)
                        {
                            // Branch node
                            if (!parent.ContainsKey(part))
                                parent.Add(part, new Dictionary<string, object>());

                            // Grab the next parent
                            parent = (Dictionary<string, object>)parent[part];
                        }
                        else
                        {
                            // Leaf node
                            parent.Add(part, kvp.Value);
                        }
                    }
                }
                else
                {
                    // Add current level directly
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            return result;
        }
    }
}
