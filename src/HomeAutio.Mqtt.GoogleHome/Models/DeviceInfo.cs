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
        public string Manufacturer { get; set; }

        /// <summary>
        /// Model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Hardware version.
        /// </summary>
        public string HwVersion { get; set; }

        /// <summary>
        /// Software version.
        /// </summary>
        public string SwVersion { get; set; }
    }
}
