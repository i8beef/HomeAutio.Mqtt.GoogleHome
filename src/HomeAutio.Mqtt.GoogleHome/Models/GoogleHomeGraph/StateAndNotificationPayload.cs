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
        public ReportStateAndNotificationDevice Devices { get; set; }
    }
}
