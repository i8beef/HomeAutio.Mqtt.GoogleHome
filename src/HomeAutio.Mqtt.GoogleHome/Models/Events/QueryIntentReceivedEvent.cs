using System;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Events
{
    /// <summary>
    /// Query intent received event.
    /// </summary>
    public class QueryIntentReceivedEvent
    {
        /// <summary>
        /// Devices.
        /// </summary>
        public IList<Request.Device> Devices { get; set; }

        /// <summary>
        /// Time of event.
        /// </summary>
        public DateTimeOffset Time { get; set; }
    }
}
