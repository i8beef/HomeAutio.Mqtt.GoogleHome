using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    /// <summary>
    /// Execute intent request object.
    /// </summary>
    public class ExecuteIntentPayload
    {
        /// <summary>
        /// Commands to execute.
        /// </summary>
        public IList<Command> Commands { get; set; }
    }
}
