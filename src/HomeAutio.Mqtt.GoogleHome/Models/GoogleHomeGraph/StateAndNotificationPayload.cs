namespace HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph
{
    /// <summary>
    /// State and notification payload request object.
    /// </summary>
    public class StateAndNotificationPayload
    {
        /// <summary>
        /// Devices.
        /// </summary>
        public required ReportStateAndNotificationDevice Devices { get; init; }
    }
}
