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
        public string AgentUserId { get; set; }

        /// <summary>
        /// Devices.
        /// </summary>
        public IList<Device> Devices { get; set; }
    }
}
