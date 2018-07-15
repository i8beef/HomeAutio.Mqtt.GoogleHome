using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Device response object.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device()
        {
            Traits = new List<string>();
        }

        /// <summary>
        /// Device id..
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Device type.
        /// </summary>
        public DeviceType Type { get; set;  }

        /// <summary>
        /// Device name information.
        /// </summary>
        public NameInfo Name { get; set; }

        /// <summary>
        /// Whether device will report state.
        /// </summary>
        public bool WillReportState { get; set; }

        /// <summary>
        /// Room hint.
        /// </summary>
        public string RoomHint { get; set; }

        /// <summary>
        /// Device information.
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// Supported traits.
        /// </summary>
        public IList<string> Traits { get; set; }

        /// <summary>
        /// Attributes.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Custom data.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> CustomData { get; set; }
    }
}
