using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// Custom converter to convert objects to and from JSON
    /// </summary>
    /// <typeparam name="T">The type of object being passed in</typeparam>
    public abstract class CustomJsonConverter<T> : JsonConverter
    {
        /// <summary>
        /// Abstract method which implements the appropriate create method
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jsonObject);

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
        public override bool CanWrite { get { return false; } }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // load the json string
            var jsonObject = JObject.Load(reader);

            // instantiate the appropriate object based on the json string
            var target = Create(objectType, jsonObject);

            // populate the properties of the object
            serializer.Populate(jsonObject.CreateReader(), target);

            // return the object
            return target;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
