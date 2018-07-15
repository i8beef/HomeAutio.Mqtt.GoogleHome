using System;
using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Newtonsoft.Json.Linq;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// The converter to use when deserializing value map objects
    /// </summary>
    public class ValueMapJsonConverter : CustomJsonConverter<MapBase>
    {
        /// <inheritdoc />
        protected override MapBase Create(Type objectType, JObject jsonObject)
        {
            // examine the intent value
            string typeName = jsonObject["type"].ToString();

            // based on the intent, instantiate and return a new object
            switch (typeName)
            {
                case "range":
                    return new RangeMap();
                case "value":
                    return new ValueMap();
                case "static":
                    return new StaticMap();
                default:
                    return null;
            }
        }
    }
}
