using HomeAutio.Mqtt.GoogleHome.Models.Request;
using Newtonsoft.Json.Linq;
using System;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// The converter to use when deserializing intent objects
    /// </summary>
    public class IntentJsonConverter : CustomJsonConverter<IntentBase>
    {
        /// <summary>
        /// The class that will create Intents when proper json objects are passed in
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        protected override IntentBase Create(Type objectType, JObject jsonObject)
        {
            // examine the intent value
            string typeName = (jsonObject["intent"]).ToString();

            // based on the intent, instantiate and return a new object
            switch (typeName)
            {
                case "action.devices.SYNC":
                    return new SyncIntent();
                case "action.devices.QUERY":
                    return new QueryIntent();
                case "action.devices.EXECUTE":
                    return new ExecuteIntent();
                case "action.devices.DISCONNECT":
                    return new DisconnectIntent();
                default:
                    return null;
            }
        }
    }
}
