using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.IntentHandlers
{
    /// <summary>
    /// Query intent handler.
    /// </summary>
    public class QueryIntentHandler
    {
        private readonly ILogger<QueryIntentHandler> _log;

        private readonly GoogleDeviceRepository _deviceRepository;
        private readonly StateCache _stateCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="stateCache">State cache.</param>
        public QueryIntentHandler(
            ILogger<QueryIntentHandler> logger,
            GoogleDeviceRepository deviceRepository,
            StateCache stateCache)
        {
            _log = logger;
            _deviceRepository = deviceRepository;
            _stateCache = stateCache;
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.QueryIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.QueryResponsePayload"/>.</returns>
        public Models.Response.QueryResponsePayload Handle(Models.Request.QueryIntent intent)
        {
            _log.LogInformation("Received QUERY intent for devices: " + string.Join(", ", intent.Payload.Devices.Select(x => x.Id)));

            var queryResponsePayload = new Models.Response.QueryResponsePayload
            {
                Devices = intent.Payload.Devices
                    .GroupBy(d => d.Id)
                    .Select(g => g.First())
                    .ToDictionary(x => x.Id, x => _deviceRepository.Get(x.Id).GetGoogleState(_stateCache))
            };

            return queryResponsePayload;
        }
    }
}
