using System.Collections.Generic;
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
        /// <returns>Validation errors.</returns>
        public static IEnumerable<string> Validate(NameInfo nameInfo)
        {
            var validationErrors = new List<string>();

            if (nameInfo is null)
            {
                validationErrors.Add("NameInfo is missing");
            }
            else
            {
                if (string.IsNullOrEmpty(nameInfo.Name))
                {
                    validationErrors.Add("NameInfo Name is missing");
                }

                if (nameInfo.DefaultNames is not null)
                {
                    if (nameInfo.DefaultNames.Any(string.IsNullOrEmpty))
                    {
                        validationErrors.Add("NameInfo DefaultNames cannot contain empty values");
                    }
                }

                if (nameInfo.Nicknames is not null)
                {
                    if (nameInfo.Nicknames.Any(string.IsNullOrEmpty))
                    {
                        validationErrors.Add("NameInfo Nicknames cannot contain empty values");
                    }
                }
            }

            return validationErrors;
        }
    }
}
