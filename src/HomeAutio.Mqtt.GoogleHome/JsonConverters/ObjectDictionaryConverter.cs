using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HomeAutio.Mqtt.GoogleHome.JsonConverters
{
    /// <summary>
    /// Object dictionary converter.
    /// </summary>
    public class ObjectDictionaryConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType) { return typeof(IDictionary<string, object>).IsAssignableFrom(objectType); }

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return ReadValue(reader);
        }

        /// <summary>
        /// Reads the current value.
        /// </summary>
        /// <param name="reader">JSON reader.</param>
        /// <returns>The read object.</returns>
        private object? ReadValue(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                {
                    throw new JsonSerializationException("Unexpected Token when converting IDictionary<string, object>");
                }
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ReadObject(reader);
                case JsonToken.StartArray:
                    return ReadArray(reader);
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return reader.Value;
                default:
                    throw new JsonSerializationException(string.Format("Unexpected token when converting IDictionary<string, object>: {0}", reader.TokenType));
            }
        }

        /// <summary>
        /// Reads the current array value.
        /// </summary>
        /// <param name="reader">JSON reader.</param>
        /// <returns>The read object.</returns>
        private object ReadArray(JsonReader reader)
        {
            IList<object?> list = new List<object?>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;
                    default:
                        var v = ReadValue(reader);

                        list.Add(v);
                        break;
                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
        }

        /// <summary>
        /// Reads the current object value.
        /// </summary>
        /// <param name="reader">JSON reader.</param>
        /// <returns>The read object.</returns>
        private object ReadObject(JsonReader reader)
        {
            var obj = new Dictionary<string, object?>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var propertyValue = reader.Value;
                        if (propertyValue is not null)
                        {
                            var propertyName = propertyValue.ToString();

                            if (!reader.Read())
                            {
                                throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
                            }

                            var v = ReadValue(reader);

                            obj[propertyName!] = v;
                        }
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return obj;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            WriteValue(writer, value);
        }

        /// <summary>
        /// Writes the current value.
        /// </summary>
        /// <param name="writer">JSON writer.</param>
        /// <param name="value">The value to write.</param>
        private void WriteValue(JsonWriter writer, object? value)
        {
            if (value != null)
            {
                var t = JToken.FromObject(value);
                switch (t.Type)
                {
                    case JTokenType.Object:
                        WriteObject(writer, value);
                        break;
                    case JTokenType.Array:
                        WriteArray(writer, value);
                        break;
                    default:
                        writer.WriteValue(value);
                        break;
                }
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        /// <summary>
        /// Writes the current object value.
        /// </summary>
        /// <param name="writer">JSON writer.</param>
        /// <param name="value">The value to write.</param>
        private void WriteObject(JsonWriter writer, object value)
        {
            writer.WriteStartObject();
            var obj = (IDictionary<string, object?>)value;
            foreach (var kvp in obj)
            {
                writer.WritePropertyName(kvp.Key);
                WriteValue(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes the current array value.
        /// </summary>
        /// <param name="writer">JSON writer.</param>
        /// <param name="value">The value to write.</param>
        private void WriteArray(JsonWriter writer, object value)
        {
            writer.WriteStartArray();
            var array = (IEnumerable<object?>)value;
            foreach (var o in array)
            {
                WriteValue(writer, o);
            }

            writer.WriteEndArray();
        }
    }
}
