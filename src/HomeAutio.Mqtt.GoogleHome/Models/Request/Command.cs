using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Command request object.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Devices to execute against.
        /// </summary>
        public IList<Device> Devices { get; set; }

        /// <summary>
        /// Execution details.
        /// </summary>
        public IList<Execution> Execution { get; set; }
    }
}
