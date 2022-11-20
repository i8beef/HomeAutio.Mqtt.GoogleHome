﻿using System;
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
            // Examine the intent value
            var typeName = jsonObject["type"].ToString();

            // Based on the intent, instantiate and return a new object
            switch (typeName)
            {
                case "celsius":
                    return new TemperatureMap();
                case "linearRange":
                    return new LinearRangeMap();
                case "range":
                    return new RangeMap();
                case "regex":
                    return new RegexMap();
                case "static":
                    return new StaticMap();
                case "value":
                    return new ValueMap();
                default:
                    return null;
            }
        }
    }
}
