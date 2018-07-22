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
            var parameters = new Dictionary<string, object>();
            foreach (var stateParam in Traits
                .Where(trait => trait.Trait != "action.devices.traits.CameraStream")
                .SelectMany(trait => trait.State)
                .Where(state => state.Value.Topic != null))
            {
                // Ignore things with no state
                if (!stateCache.ContainsKey(stateParam.Value.Topic))
                    continue;

                // Convert state
                var value = stateCache[stateParam.Value.Topic];
                if (stateParam.Key.Contains('.'))
                {
                    // Parameter is a cmplex object
                    var complexParameterParts = stateParam.Key.Split('.');
                    if (complexParameterParts.Count() > 2)
                        throw new System.Exception("Status key contained more than one '.'");

                    // Ensure root key is present
                    if (!parameters.Keys.Contains(complexParameterParts[0]))
                        parameters.Add(complexParameterParts[0], new Dictionary<string, object>());

                    // Add parameter
                    var complexValue = (IDictionary<string, object>)parameters[complexParameterParts[0]];
                    complexValue.Add(complexParameterParts[1], stateParam.Value.MapValueToGoogle(stateParam.Key, value));
                }
                else
                {
                    parameters.Add(stateParam.Key, stateParam.Value.MapValueToGoogle(stateParam.Key, value));
                }
            }

            return parameters as IDictionary<string, object>;
        }
    }
}
