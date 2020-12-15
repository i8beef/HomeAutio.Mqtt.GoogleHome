using HomeAutio.Mqtt.GoogleHome.Models.Request;

namespace HomeAutio.Mqtt.GoogleHome.Models.Events
{
    /// <summary>
    /// Device command execution event.
    /// </summary>
    public class DeviceCommandExecutionEvent
    {
        /// <summary>
        /// Device id.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Execution.
        /// </summary>
        public Execution Execution { get; set; }
    }
}
