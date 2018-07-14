using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class ExecuteIntentPayload
    {
        public IList<Command> Commands { get; set; }
    }
}
