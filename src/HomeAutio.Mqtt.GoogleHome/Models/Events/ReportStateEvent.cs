using System.Collections.Generic;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome.Models.Events
{
    /// <summary>
    /// ReportStateAndNotification event.
    /// </summary>
    public class ReportStateEvent
    {
        /// <summary>
        /// Devices to report state for.
        /// </summary>
        public IList<Device> Devices { get; set; }
    }
}
