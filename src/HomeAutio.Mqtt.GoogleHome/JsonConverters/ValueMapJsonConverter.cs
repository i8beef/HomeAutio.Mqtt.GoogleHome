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
        protected override Type? GetTargetType(Type objectType, JObject jsonObject)
        {
            // Examine the intent value
            var typeName = jsonObject["type"]?.ToString();

            // Based on the intent, instantiate and return a new object
            switch (typeName)
            {
                case "celsius":
                    return typeof(TemperatureMap);
                case "linearRange":
                    return typeof(LinearRangeMap);
                case "range":
                    return typeof(RangeMap);
                case "regex":
                    return typeof(RegexMap);
                case "static":
                    return typeof(StaticMap);
                case "value":
                    return typeof(ValueMap);
                default:
                    return null;
            }
        }
    }
}
