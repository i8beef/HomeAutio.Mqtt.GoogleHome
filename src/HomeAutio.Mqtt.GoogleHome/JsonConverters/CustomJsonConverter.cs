using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// Custom converter to convert objects to and from JSON
    /// </summary>
    /// <typeparam name="T">The type of object being passed in</typeparam>
    public abstract class CustomJsonConverter<T> : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Abstract method which implements the appropriate create method
        /// </summary>
        /// <param name="objectType">The type of object to create.</param>
        /// <param name="jsonObject">The source JSON object.</param>
        /// <returns>An instance of the specified type.</returns>
        protected abstract T Create(Type objectType, JObject jsonObject);

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Undefined:
                case JsonToken.Null:
                    return null;
            }

            // Load the json string
            var jsonObject = JObject.Load(reader);

            // Instantiate the appropriate object based on the json string
            var target = Create(objectType, jsonObject);

            if (target == null)
                return null;

            // Populate the properties of the object
            serializer.Populate(jsonObject.CreateReader(), target);

            return target;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
