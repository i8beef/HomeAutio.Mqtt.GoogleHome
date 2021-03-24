using System;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.IntentHandlers
{
    /// <summary>
    /// Sync intent handler.
    /// </summary>
    public class DisconnectIntentHandler
    {
        private readonly ILogger<DisconnectIntentHandler> _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        public DisconnectIntentHandler(ILogger<DisconnectIntentHandler> logger)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.DisconnectIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        public void Handle(Models.Request.DisconnectIntent intent)
        {
            // Only log for now as disconnect isn't supported
            _log.LogInformation("Received DISCONNECT intent");
        }
    }
}
