using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph
{
    /// <summary>
    /// Devices request object.
    /// </summary>
    public class ReportStateAndNotificationDevice
    {
        /// <summary>
        /// States of devices to update.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(ObjectDictionaryConverter))]
        public IDictionary<string, IDictionary<string, object?>>? States { get; init; }

        /// <summary>
        /// Notifications metadata for devices.
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(ObjectDictionaryConverter))]
        public IDictionary<string, IDictionary<string, object?>>? Notifications { get; init; }
    }
}
