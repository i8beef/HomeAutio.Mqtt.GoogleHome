using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets device state as a Google device state object.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>A Google device state object.</returns>
        public IDictionary<string, object> GetGoogleState(IDictionary<string, string> stateCache)
        {
            var stateConfigs = Traits
                .Where(trait => trait.Trait != TraitType.CameraStream)
                .SelectMany(trait => trait.State)
                .Where(state => state.Value.Topic != null)
                .Where(state => stateCache.ContainsKey(state.Value.Topic))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.MapValueToGoogle(stateCache[kvp.Value.Topic]))
                .ToNestedDictionary();

            return stateConfigs;
        }
    }
}
