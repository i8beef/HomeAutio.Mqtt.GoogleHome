using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Execution response payload response object.
    /// </summary>
    public class ExecutionResponsePayload
    {
        /// <summary>
        /// Commands.
        /// </summary>
        public required IList<Command> Commands { get; init; }
    }
}
