using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    /// <summary>
    /// Readonly device configuration.
    /// </summary>
    public class DeviceConfiguration : ReadOnlyDictionary<string, Device>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceConfiguration"/> class.
        /// </summary>
        public DeviceConfiguration()
            : this(new Dictionary<string, Device>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceConfiguration"/> class.
        /// </summary>
        /// <param name="dictionary">The deserialized device information dictionary.</param>
        public DeviceConfiguration(IDictionary<string, Device> dictionary)
            : base(dictionary)
        {
        }
    }
}
