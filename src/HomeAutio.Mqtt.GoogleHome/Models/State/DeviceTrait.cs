using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using HomeAutio.Mqtt.GoogleHome.Models.State.Challenges;
using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    /// <summary>
    /// Device trait configuration.
    /// </summary>
    public class DeviceTrait
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceTrait"/> class.
        /// </summary>
        public DeviceTrait()
        {
            Commands = new Dictionary<string, IDictionary<string, string>>();
            State = new Dictionary<string, DeviceState>();
        }

        /// <summary>
        /// Trait name.
        /// </summary>
        public TraitType Trait { get; set; }

        /// <summary>
        /// Trait supporting attributes.
        /// </summary>
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Trait supported commands.
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> Commands { get; set; }

        /// <summary>
        /// Trait state configuration.
        /// </summary>
        public IDictionary<string, DeviceState> State { get; set; }

        /// <summary>
        /// Challenges for executing trait commands.
        /// </summary>
        [JsonConverter(typeof(ChallengeJsonConverter))]
        public ChallengeBase Challenge { get; set; }

        /// <summary>
        /// Gets trait state as a Google device state object.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>A Google device state object.</returns>
        public IDictionary<string, object> GetGoogleState(IDictionary<string, string> stateCache)
        {
            return GetGoogleStateFlattened(stateCache).ToNestedDictionary();
        }

        /// <summary>
        /// Gets trait state as a Google device state object in a flattened state.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>A Google device state object in a flattened state.</returns>
        public IDictionary<string, object> GetGoogleStateFlattened(IDictionary<string, string> stateCache)
        {
            var result = new Dictionary<string, object>();

            if (State != null)
            {
                foreach (var state in State)
                {
                    if (state.Value.Topic != null && stateCache.ContainsKey(state.Value.Topic))
                    {
                        result.Add(state.Key, state.Value.MapValueToGoogle(stateCache[state.Value.Topic]));
                    }
                    else if (state.Value.Topic == null && state.Value.ValueMap != null && state.Value.ValueMap.Any(x => x is StaticMap))
                    {
                        result.Add(state.Key, state.Value.MapValueToGoogle(null));
                    }
                }
            }

            return result;
        }
    }
}
