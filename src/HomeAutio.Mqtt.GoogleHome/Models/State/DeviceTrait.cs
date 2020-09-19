using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using HomeAutio.Mqtt.GoogleHome.Models.State.Challenges;
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
    }
}
