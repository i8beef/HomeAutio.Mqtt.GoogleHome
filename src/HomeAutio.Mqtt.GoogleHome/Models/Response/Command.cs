using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Response
{
    public class Command
    {
        public Command()
        {
            Ids = new List<string>();
        }

        public IList<string> Ids { get; set; }
        public CommandStatus Status { get; set; }

        [JsonConverter(typeof(ObjectDictionaryConverter))]
        public IDictionary<string, object> States { get; set; }

        public string DebugString { get; set; }
        public string ErrorCode { get; set; }
    }
}
