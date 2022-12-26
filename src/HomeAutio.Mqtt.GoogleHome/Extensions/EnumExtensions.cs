using System;
using System.Linq;
using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Extensions
{
    /// <summary>
    /// Enum extensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets <see cref="EnumMemberAttribute"/> value for an enum value.
        /// </summary>
        /// <param name="enumValue">Enum value to convert.</param>
        /// <returns>The <see cref="EnumMemberAttribute"/> value.</returns>
        public static string ToEnumString(this Enum enumValue)
        {
            var enumMemberAttribute = enumValue
                .GetType()
                .GetField(enumValue.ToString())?
                .GetCustomAttributes(typeof(EnumMemberAttribute), true)
                .Single() as EnumMemberAttribute;

            return enumMemberAttribute?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets an enum value by its <see cref="EnumMemberAttribute"/> value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="str">String to convert.</param>
        /// <returns>The enum value.</returns>
        public static T? ToEnum<T>(this string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumFieldInfo = enumType.GetField(name);
                if (enumFieldInfo is not null)
                {
                    var enumMemberAttribute = enumFieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)
                        .Cast<EnumMemberAttribute>()
                        .SingleOrDefault();

                    if (enumMemberAttribute is not null && enumMemberAttribute.Value == str)
                    {
                        return (T)Enum.Parse(enumType, name);
                    }
                }
            }

            return default;
        }
    }
}
