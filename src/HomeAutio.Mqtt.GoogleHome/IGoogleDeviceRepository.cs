using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Google device repository.
    /// </summary>
    public interface IGoogleDeviceRepository
    {
        /// <summary>
        /// Adds a device.
        /// </summary>
        /// <param name="device">Device to add.</param>
        void Add(Device device);

        /// <summary>
        /// Determines if the device is contained in the repository.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <returns><c>true</c> if present, else <c>false</c>.</returns>
        bool Contains(string deviceId);

        /// <summary>
        /// Deletes a device.
        /// </summary>
        /// <param name="deviceId">Device Id to delete.</param>
        void Delete(string deviceId);

        /// <summary>
        /// Gets a device by id.
        /// </summary>
        /// <param name="deviceId">Device Id to get.</param>
        /// <returns>The <see cref="Device"/>.</returns>
        Device? FindById(string deviceId);

        /// <summary>
        /// Gets all devices.
        /// </summary>
        /// <returns>A list of <see cref="Device"/>.</returns>
        IList<Device> GetAll();

        /// <summary>
        /// Gets a device detached from the repository.
        /// </summary>
        /// <param name="deviceId">Device Id to get.</param>
        /// <returns>The detached <see cref="Device"/>.</returns>
        Device GetDetached(string deviceId);

        /// <summary>
        /// Refreshes deice configuration from file.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Updates a device.
        /// </summary>
        /// <param name="deviceId">Device ID to update.</param>
        /// <param name="device">Device to update.</param>
        void Update(string deviceId, Device device);
    }
}
