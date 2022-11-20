using System;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using Newtonsoft.Json.Linq;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// The converter to use when deserializing intent objects
    /// </summary>
    public class IntentJsonConverter : CustomJsonConverter<IntentBase>
    {
        /// <inheritdoc />
        protected override IntentBase Create(Type objectType, JObject jsonObject)
        {
            // Examine the intent value
            var typeName = jsonObject["intent"].ToString();

            // Based on the intent, instantiate and return a new object
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
