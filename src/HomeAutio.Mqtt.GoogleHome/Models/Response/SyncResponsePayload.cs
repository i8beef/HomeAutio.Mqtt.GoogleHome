using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Sync response payload response object.
    /// </summary>
    public class SyncResponsePayload
    {
        /// <summary>
        /// Agent user id.
        /// </summary>
        public required string AgentUserId { get; init; }

        /// <summary>
        /// Devices.
        /// </summary>
        public required IList<Device> Devices { get; init; }
    }
}
