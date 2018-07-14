using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    public class Device
    {
        public Device()
        {
            Traits = new List<string>();
        }

        public string Id { get; set; }
        public DeviceType Type { get; set;  }
        public NameInfo Name { get; set; }
        public bool WillReportState { get; set; }
        public string RoomHint { get; set; }
        public DeviceInfo DeviceInfo { get; set; }

        public IList<string> Traits { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Attributes { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> CustomData { get; set; }
    }
}
