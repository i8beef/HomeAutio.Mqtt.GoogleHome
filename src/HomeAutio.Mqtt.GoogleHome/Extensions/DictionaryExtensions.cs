using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeAutio.Mqtt.GoogleHome.Extensions
{
    /// <summary>
    /// Dictionary extensions.
    /// </summary>
    public static partial class DictionaryExtensions
    {
        /// <summary>
        /// Flattens a nested dictionary with keys joined by the specified delimiter.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <param name="delimiter">Delimiter.</param>
        /// <returns>A flattened Dictionary.</returns>
        public static IDictionary<string, object?> ToFlatDictionary(this IDictionary<string, object?> source, string delimiter = ".")
        {
            var result = new Dictionary<string, object?>();
            foreach (var kvp in source)
            {
                if (kvp.Value is IDictionary<string, object?> dictionary)
                {
                    // Objects
                    var flattenedValueDictionary = dictionary.ToFlatDictionary(delimiter);
                    foreach (var subKvp in flattenedValueDictionary)
                    {
                        result.Add(string.Join(delimiter, kvp.Key, subKvp.Key), subKvp.Value);
                    }
                }
                else if (kvp.Value is IList<object> list)
                {
                    // Arrays
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i] is IDictionary<string, object?> itemDictionary)
                        {
                            var flattenedValueDictionary = itemDictionary.ToFlatDictionary(delimiter);
                            foreach (var subKvp in flattenedValueDictionary)
                            {
                                result.Add(string.Join(delimiter, kvp.Key, $"[{i}]", subKvp.Key), subKvp.Value);
                            }
                        }
                        else
                        {
                            result.Add(string.Join(delimiter, kvp.Key, $"[{i}]"), list[i]);
                        }
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
        public static IDictionary<string, object?> ToNestedDictionary(this IDictionary<string, object?> source, string delimiter = ".")
        {
            var result = new Dictionary<string, object?>();
            foreach (var kvp in source)
            {
                if (kvp.Value is IDictionary<string, object?>)
                {
                    throw new System.FormatException("Cannot convert a non-flat Dictionary to a nested dictionary.");
                }

                if (kvp.Key.Contains(delimiter))
                {
                    // Split and recursively add
                    var parts = new Queue<string>(kvp.Key.Split(delimiter));

                    // Start with the root result object
                    var currentContainerNode = result;
                    while (parts.Count > 0)
                    {
                        var part = parts.Dequeue();
                        if (parts.Count > 0)
                        {
                            // Branch node exists check
                            if (!currentContainerNode.ContainsKey(part))
                            {
                                currentContainerNode.Add(part, new Dictionary<string, object?>());
                            }

                            // Traverse to the next level container node
                            currentContainerNode = (Dictionary<string, object?>)currentContainerNode[part]!;
                        }
                        else
                        {
                            // Leaf node
                            currentContainerNode.Add(part, kvp.Value);
                        }
                    }
                }
                else
                {
                    // Add current level directly
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            // Convert arrays
            return result.ConvertArrayKeys();
        }

        /// <summary>
        /// Recusively converts array structures in a Dictionary heirarchy.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <returns>A flattened Dictionary.</returns>
        public static IDictionary<string, object?> ConvertArrayKeys(this IDictionary<string, object?> source)
        {
            var result = new Dictionary<string, object?>();

            foreach (var kvp in source)
            {
                if (kvp.Value is Dictionary<string, object?> valueAsDictionary)
                {
                    if (valueAsDictionary.Keys.All(x => ArrayRegex().IsMatch(x)))
                    {
                        var list = valueAsDictionary.Values.ToList();

                        result.Add(kvp.Key, list);
                    }
                    else
                    {
                        // Recursive convert
                        result.Add(kvp.Key, valueAsDictionary.ConvertArrayKeys());
                    }
                }
                else
                {
                    // Add leaf as is
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            return result;
        }

        [GeneratedRegex(@"^\[\d+\]$")]
        private static partial Regex ArrayRegex();
    }
}
