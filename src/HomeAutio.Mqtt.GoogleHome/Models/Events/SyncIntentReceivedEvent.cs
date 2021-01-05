using System;

namespace HomeAutio.Mqtt.GoogleHome.Models.Events
{
    /// <summary>
    /// Sync intent received event.
    /// </summary>
    public class SyncIntentReceivedEvent
    {
        /// <summary>
        /// Time of event.
        /// </summary>
        public DateTimeOffset Time { get; set; }
    }
}
