using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    /// <summary>
    /// Device state configuration.
    /// </summary>
    public class DeviceState
    {
        /// <summary>
        /// MQTT topic.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Google device state.
        /// </summary>
        public GoogleType GoogleType { get; set; }

        /// <summary>
        /// Value mappings.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(ValueMapJsonConverter))]
        public IList<MapBase> ValueMap { get; set; }
    }
}
