namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device configuration element.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Manufacturer.
        /// </summary>
        public string? Manufacturer { get; init; }

        /// <summary>
        /// Model.
        /// </summary>
        public string? Model { get; init; }

        /// <summary>
        /// Hardware version.
        /// </summary>
        public string? HwVersion { get; init; }

        /// <summary>
        /// Software version.
        /// </summary>
        public string? SwVersion { get; init; }
    }
}
