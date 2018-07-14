using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class Command
    {
        public IList<Device> Devices { get; set; }
        public IList<Execution> Execution { get; set; }
    }
}
