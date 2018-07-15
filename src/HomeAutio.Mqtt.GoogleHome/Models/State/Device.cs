using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    /// <summary>
    /// Device.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Device id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Device type.
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// Indicates if the device will report state.
        /// </summary>
        public bool WillReportState { get; set; }

        /// <summary>
        /// Room hint.
        /// </summary>
        public string RoomHint { get; set; }

        /// <summary>
        /// Device name information.
        /// </summary>
        public NameInfo Name { get; set; }

        /// <summary>
        /// Device information.
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// Trait configurations.
        /// </summary>
        public IList<DeviceTrait> Traits { get; set; }

        /// <summary>
        /// Custom data.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> CustomData { get; set; }
    }
}
