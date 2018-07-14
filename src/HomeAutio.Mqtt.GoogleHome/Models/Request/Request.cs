using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.Request
{
    public class Request
    {
        public string RequestId { get; set; }

        [JsonProperty(ItemConverterType = typeof(IntentJsonConverter))]
        public IList<IntentBase> Inputs { get; set; }
    }
}
