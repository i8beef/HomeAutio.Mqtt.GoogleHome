using System;
using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Exceptions
{
    /// <summary>
    /// GoogleHomeGraph auth exception.
    /// </summary>
    public class GoogleHomeGraphAuthException : Exception
    {
        /// <inheritdoc/>
        public GoogleHomeGraphAuthException()
        {
        }

        /// <inheritdoc/>
        public GoogleHomeGraphAuthException(string? message) : base(message)
        {
        }

        /// <inheritdoc/>
        public GoogleHomeGraphAuthException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        protected GoogleHomeGraphAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
