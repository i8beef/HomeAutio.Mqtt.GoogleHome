using System;
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
        public static void Validate(Device device)
        {
            if (string.IsNullOrEmpty(device.Id))
                throw new Exception("Device Id is missing");

            if (device.Type == Models.DeviceType.Unknown)
                throw new Exception("Device Type is missing or not a valid type");

            DeviceInfoValidator.Validate(device.DeviceInfo);
            NameInfoValidator.Validate(device.Name);
            CustomDataValidator.Validate(device.CustomData);

            foreach (var trait in device.Traits)
            {
                DeviceTraitValidator.Validate(trait);
            }
        }
    }
}
