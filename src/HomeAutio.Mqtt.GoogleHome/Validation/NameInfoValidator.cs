using System;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// <see cref="NameInfo"/> validator.
    /// </summary>
    public static class NameInfoValidator
    {
        /// <summary>
        /// Validates a <see cref="NameInfo"/>.
        /// </summary>
        /// <param name="nameInfo">The <see cref="NameInfo"/> to validate.</param>
        public static void Validate(NameInfo nameInfo)
        {
            if (nameInfo.Name == null)
                throw new Exception("NameInfo Name is missing");

            if (nameInfo.DefaultNames != null)
            {
                if (nameInfo.DefaultNames.Any(x => string.IsNullOrEmpty(x)))
                    throw new Exception("NameInfo DefaultNames cannot contain empty values");
            }

            if (nameInfo.Nicknames != null)
            {
                if (nameInfo.Nicknames.Any(x => string.IsNullOrEmpty(x)))
                    throw new Exception("NameInfo Nicknames cannot contain empty values");
            }
        }
    }
}
