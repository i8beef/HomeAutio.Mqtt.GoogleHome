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
        public required string DeviceId { get; init; }

        /// <summary>
        /// Execution.
        /// </summary>
        public required Execution Execution { get; init; }
    }
}
