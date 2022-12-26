using System;
using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Extensions
{
    /// <summary>
    /// Configuration extensions.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <returns>The converted value.</returns>
        public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        {
            var value = configuration.GetValue<T>(key);
            if (value is null)
            {
                throw new InvalidOperationException($"Configuration value {key} not found");
            }
            else
            {
                return value;
            }
        }
    }
}
