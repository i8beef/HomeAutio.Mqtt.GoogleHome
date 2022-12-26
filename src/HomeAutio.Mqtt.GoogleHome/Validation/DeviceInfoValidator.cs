using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// <see cref="DeviceInfo"/> validator.
    /// </summary>
    public static class DeviceInfoValidator
    {
        /// <summary>
        /// Validates a <see cref="DeviceInfo"/>.
        /// </summary>
        /// <param name="deviceInfo">The <see cref="DeviceInfo"/> to validate.</param>
        /// <returns>Validation errors.</returns>
        public static IEnumerable<string> Validate(DeviceInfo? deviceInfo)
        {
            return Enumerable.Empty<string>();
        }
    }
}
