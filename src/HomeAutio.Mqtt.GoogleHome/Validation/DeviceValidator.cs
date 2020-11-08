using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models;
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
            {
                validationErrors.Add($"Device Id is missing for Device of type {device.Type}");
                return validationErrors;
            }

            if (device.Type == DeviceType.Unknown)
            {
                validationErrors.Add($"Device Type is missing or not a valid type on Device id {device.Id}");
                return validationErrors;
            }

            validationErrors.AddRange(DeviceInfoValidator.Validate(device.DeviceInfo));
            validationErrors.AddRange(NameInfoValidator.Validate(device.Name));
            validationErrors.AddRange(CustomDataValidator.Validate(device.CustomData));

            if (device.Traits != null)
            {
                foreach (var trait in device.Traits)
                {
                    validationErrors.AddRange(DeviceTraitValidator.Validate(trait).Select(x => $"{x} on Device id {device.Id}"));
                }
            }

            return validationErrors;
        }
    }
}
