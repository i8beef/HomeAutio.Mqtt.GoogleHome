using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class QueryIntentPayload
    {
        public IList<Device> Devices { get; set; }
    }
}
