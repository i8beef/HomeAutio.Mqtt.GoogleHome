using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public class DeviceState
    {
        public string Topic { get; set; }
        public GoogleType GoogleType { get; set; }

        [JsonProperty(ItemConverterType = typeof(ValueMapJsonConverter))]
        public IList<MapBase> ValueMap { get; set; }
    }
}
