using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
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
        /// Indicates if device is disabled or not.
        /// </summary>
        public bool Disabled { get; set; }

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
            return GetGoogleStateFlattened(stateCache).ToNestedDictionary();
        }

        /// <summary>
        /// Gets device state as a Google device state object for use in QUERY responses.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>A Google device state object for use in QUERY responses.</returns>
        public IDictionary<string, object> GetGoogleQueryState(IDictionary<string, string> stateCache)
        {
            var stateConfigs = GetGoogleStateFlattened(stateCache);
            var filteredStateConfigs = new Dictionary<string, object>();
            foreach (var key in stateConfigs.Keys)
            {
                switch (key)
                {
                    case "color.spectrumRGB":
                        filteredStateConfigs.Add("color.spectrumRgb", stateConfigs[key]);
                        break;
                    case "color.spectrumHSV.hue":
                        filteredStateConfigs.Add("color.spectrumHsv.hue", stateConfigs[key]);
                        break;
                    case "color.spectrumHSV.saturation":
                        filteredStateConfigs.Add("color.spectrumHsv.saturation", stateConfigs[key]);
                        break;
                    case "color.spectrumHSV.value":
                        filteredStateConfigs.Add("color.spectrumHsv.value", stateConfigs[key]);
                        break;
                    default:
                        filteredStateConfigs.Add(key, stateConfigs[key]);
                        break;
                }
            }

            return filteredStateConfigs.ToNestedDictionary();
        }

        /// <summary>
        /// Checks if the state cache for the device has been fully initialized.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns><c>true</c> if fully initialized, else <c>false</c>.</returns>
        public bool IsStateFullyInitialized(IDictionary<string, string> stateCache)
        {
            return !Traits
                .Where(trait => trait.Trait != TraitType.CameraStream)
                .SelectMany(trait => trait.State)
                .Where(state => state.Value.Topic != null)
                .Where(state => stateCache.ContainsKey(state.Value.Topic))
                .Where(state => stateCache[state.Value.Topic] == null)
                .Any();
        }

        /// <summary>
        /// Gets device state as a Google device state object in a flattened state.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>A Google device state object in a flattened state.</returns>
        private IDictionary<string, object> GetGoogleStateFlattened(IDictionary<string, string> stateCache)
        {
            var stateConfigs = Traits
                .Where(trait => trait.Trait != TraitType.CameraStream)
                .SelectMany(trait => trait.State);

            var result = new Dictionary<string, object>();
            foreach (var state in stateConfigs)
            {
                if (state.Value.Topic != null && stateCache.ContainsKey(state.Value.Topic))
                {
                    result.Add(state.Key, state.Value.MapValueToGoogle(stateCache[state.Value.Topic]));
                }
                else if (state.Value.Topic == null && state.Value.ValueMap.Any(x => x is StaticMap))
                {
                    result.Add(state.Key, state.Value.MapValueToGoogle(null));
                }
            }

            return result;
        }
    }
}
