using System;
using HomeAutio.Mqtt.GoogleHome.Models.State.Challenges;
using Newtonsoft.Json.Linq;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// The converter to use when deserializing value map objects
    /// </summary>
    public class ChallengeJsonConverter : CustomJsonConverter<ChallengeBase>
    {
        /// <inheritdoc />
        protected override ChallengeBase Create(Type objectType, JObject jsonObject)
        {
            // Examine the intent value
            string typeName = jsonObject["type"].ToString();

            // Based on the intent, instantiate and return a new object
            switch (typeName)
            {
                case "ack":
                    return new AcknowledgeChallenge();
                case "pin":
                    return new PinChallenge();
                case "none":
                default:
                    return null;
            }
        }
    }
}
