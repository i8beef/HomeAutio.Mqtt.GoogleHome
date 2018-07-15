using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    /// <summary>
    /// Execution response payload response object.
    /// </summary>
    public class ExecutionResponsePayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResponsePayload"/> class.
        /// </summary>
        public ExecutionResponsePayload()
        {
            Commands = new List<Command>();
        }

        /// <summary>
        /// Commands.
        /// </summary>
        public IList<Command> Commands { get; set; }
    }
}
