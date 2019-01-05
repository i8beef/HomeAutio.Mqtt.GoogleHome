using System.Collections.Generic;
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

            // Payload for disabled a or missing device
            var offlineDeviceState = new Dictionary<string, object>
            {
                { "errorCode", "deviceNotFound" },
                { "online", false }
            };

            var queryResponsePayload = new Models.Response.QueryResponsePayload
            {
                Devices = intent.Payload.Devices
                    .GroupBy(device => device.Id)
                    .Select(group => group.First())
                    .ToDictionary(
                        singleDevice => singleDevice.Id,
                        singleDevice =>
                        {
                            var device = _deviceRepository.Get(singleDevice.Id);
                            return device != null && !device.Disabled
                                ? device.GetGoogleState(_stateCache)
                                : offlineDeviceState;
                        })
            };

            return queryResponsePayload;
        }
    }
}
