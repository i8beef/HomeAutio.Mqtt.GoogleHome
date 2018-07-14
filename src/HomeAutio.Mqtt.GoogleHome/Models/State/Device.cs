using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public class Device
    {
        public string Id { get; set; }
        public DeviceType Type { get; set; }
        public bool WillReportState { get; set; }
        public string RoomHint { get; set; }
        public NameInfo Name { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
        public IList<DeviceTrait> Traits { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> CustomData { get; set; }
    }
}
