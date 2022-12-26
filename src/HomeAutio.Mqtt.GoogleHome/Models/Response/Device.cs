using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Device response object.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Device id..
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Device type.
        /// </summary>
        public required DeviceType Type { get; init; }

        /// <summary>
        /// Device name information.
        /// </summary>
        public required NameInfo Name { get; init; }

        /// <summary>
        /// Whether device will report state.
        /// </summary>
        public required bool WillReportState { get; init; }

        /// <summary>
        /// Room hint.
        /// </summary>
        public string? RoomHint { get; init; }

        /// <summary>
        /// Device information.
        /// </summary>
        public DeviceInfo? DeviceInfo { get; init; }

        /// <summary>
        /// Supported traits.
        /// </summary>
        public required IList<TraitType> Traits { get; init; }

        /// <summary>
        /// Attributes.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object>? Attributes { get; init; }

        /// <summary>
        /// Custom data.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object>? CustomData { get; init; }
    }
}
