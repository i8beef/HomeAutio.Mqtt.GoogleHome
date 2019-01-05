using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Events
{
    /// <summary>
    /// Event for config subscriptions changes.
    /// </summary>
    public class ConfigSubscriptionChangeEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSubscriptionChangeEvent"/> class.
        /// </summary>
        public ConfigSubscriptionChangeEvent()
        {
            AddedSubscriptions = new List<string>();
            DeletedSubscriptions = new List<string>();
        }

        /// <summary>
        /// Added subscriptions.
        /// </summary>
        public IEnumerable<string> AddedSubscriptions { get; set; }

        /// <summary>
        /// Deleted subscriptions.
        /// </summary>
        public IEnumerable<string> DeletedSubscriptions { get; set; }
    }
}
