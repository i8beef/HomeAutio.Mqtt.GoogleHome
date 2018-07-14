using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class Device
    {
        public string Id { get; set; }
        public IDictionary<string, object> CustomData { get; set; }
    }
}
