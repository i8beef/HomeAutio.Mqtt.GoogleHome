using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Request response logging middlware.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request delegate.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        /// <summary>
        /// Middleware invocation.
        /// </summary>
        /// <param name="context">OWIN context.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task Invoke(HttpContext context)
        {
            var isGoogleRequest = context.Request.Path.HasValue && context.Request.Path.Value.EndsWith("/smarthome");
            var isTraceLoggingEnabled = _logger.IsEnabled(LogLevel.Trace);
            if (isGoogleRequest && isTraceLoggingEnabled)
            {
                await LogRequest(context);
                await LogResponse(context);
            }
            else
            {
                await _next(context);
            }
        }

        /// <summary>
        /// Read stream in chunks.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <returns>Read string.</returns>
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using (var textWriter = new StringWriter())
            using (var reader = new StreamReader(stream))
            {
                var readChunk = new char[readChunkBufferLength];
                int readChunkLength;

                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                }
                while (readChunkLength > 0);

                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Log request.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);
                _logger.LogTrace($"Http Request Body: {ReadStreamInChunks(requestStream)}");
                context.Request.Body.Position = 0;
            }
        }

        /// <summary>
        /// Log response.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogTrace($"Http Response: {text}");

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
