using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    public class SyncResponsePayload
    {
        public string AgentUserId { get; set; }
        public IList<Device> Devices { get; set; }
    }
}
