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
        public required IList<Device> Devices { get; init; }

        /// <summary>
        /// Execution details.
        /// </summary>
        public required IList<Execution> Execution { get; init; }
    }
}
