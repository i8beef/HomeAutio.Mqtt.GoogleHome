using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    public class ExecutionResponsePayload
    {
        public ExecutionResponsePayload()
        {
            Commands = new List<Command>();
        }

        public IList<Command> Commands { get; set; }
    }
}
