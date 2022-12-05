using System.Collections.Generic;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Extensions;
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
            var results = new Dictionary<string, object>();
            var schemas = TraitSchemaProvider.GetTraitSchemas();
            foreach (var trait in Traits)
            {
                // Dont include "stateless" traits
                var schema = schemas.FirstOrDefault(x => x.Trait == trait.Trait);
                if (schema?.StateSchema == null)
                {
                    continue;
                }

                var validState = trait.GetGoogleStateFlattened(stateCache, schema)
                    .Where(kvp => schema.StateSchema.Validator.FlattenedPathExists(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                foreach (var googleState in validState.ToNestedDictionary())
                {
                    // Note: there explicitly shouldnt be overlap here requiring TryAdd
                    results.Add(googleState.Key, googleState.Value);
                }
            }

            // Add explicit online if not specified by state mappings
            if (!results.ContainsKey("online"))
            {
                results.Add("online", true);
            }

            return results;
        }

        /// <summary>
        /// Checks if the state cache for the device has been fully initialized.
        /// </summary>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns><c>true</c> if fully initialized, else <c>false</c>.</returns>
        public bool IsStateFullyInitialized(IDictionary<string, string> stateCache)
        {
            return !Traits
                .Where(trait => trait.State != null)
                .SelectMany(trait => trait.State)
                .Where(state => state.Value.Topic != null)
                .Where(state => stateCache.ContainsKey(state.Value.Topic))
                .Any(state => stateCache[state.Value.Topic] == null);
        }
    }
}
