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
        public required IList<Request.Device> Devices { get; init; }

        /// <summary>
        /// Time of event.
        /// </summary>
        public required DateTimeOffset Time { get; init; }
    }
}
