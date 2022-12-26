using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Query intent payload request object.
    /// </summary>
    public class QueryIntentPayload
    {
        /// <summary>
        /// Devices to query.
        /// </summary>
        public required IList<Device> Devices { get; init; }
    }
}
