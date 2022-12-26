using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Events
{
    /// <summary>
    /// Event for config subscriptions changes.
    /// </summary>
    public class ConfigSubscriptionChangeEvent
    {
        /// <summary>
        /// Added subscriptions.
        /// </summary>
        public IEnumerable<string> AddedSubscriptions { get; set; } = new List<string>();

        /// <summary>
        /// Deleted subscriptions.
        /// </summary>
        public IEnumerable<string> DeletedSubscriptions { get; set; } = new List<string>();
    }
}
