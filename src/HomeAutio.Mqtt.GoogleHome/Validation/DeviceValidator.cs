using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// Device validator.
    /// </summary>
    public static class DeviceValidator
    {
        /// <summary>
        /// Validates a <see cref="Device"/>.
        /// </summary>
        /// <param name="device">Device to validate.</param>
        /// <returns>Validation errors.</returns>
        public static IEnumerable<string> Validate(Device device)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrEmpty(device.Id))
                validationErrors.Add("Device Id is missing");

            if (device.Type == Models.DeviceType.Unknown)
                validationErrors.Add("Device Type is missing or not a valid type");

            validationErrors.AddRange(DeviceInfoValidator.Validate(device.DeviceInfo));
            validationErrors.AddRange(NameInfoValidator.Validate(device.Name));
            validationErrors.AddRange(CustomDataValidator.Validate(device.CustomData));

            if (device.Traits != null)
            {
                foreach (var trait in device.Traits)
                {
                    validationErrors.AddRange(DeviceTraitValidator.Validate(trait));
                }
            }

            return validationErrors;
        }
    }
}
