using System;
using System.Collections.Generic;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
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

        private readonly IMessageHub _messageHub;
        private readonly IGoogleDeviceRepository _deviceRepository;
        private readonly StateCache _stateCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message hub.</param>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="stateCache">State cache.</param>
        public QueryIntentHandler(
            ILogger<QueryIntentHandler> logger,
            IMessageHub messageHub,
            IGoogleDeviceRepository deviceRepository,
            StateCache stateCache)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentNullException(nameof(messageHub));
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _stateCache = stateCache ?? throw new ArgumentNullException(nameof(stateCache));
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
            var offlineDeviceState = new Dictionary<string, object?>
            {
                { "errorCode", "deviceNotFound" },
                { "online", false },
                { "status", "ERROR" }
            };

            // Get distinct devices
            var distinctRequestDeviceIds = intent.Payload.Devices
                    .GroupBy(device => device.Id)
                    .Select(group => group.First())
                    .Select(device => device.Id);

            // Convert to an event to publish
            var commandEvent = new QueryIntentReceivedEvent { Devices = intent.Payload.Devices, Time = DateTimeOffset.Now };
            _messageHub.Publish(commandEvent);

            var devices = _deviceRepository.GetAll()
                .Where(device => !device.Disabled)
                .Where(device => distinctRequestDeviceIds.Contains(device.Id))
                .ToList();

            // Build reponse payload
            var queryResponsePayload = new Models.Response.QueryResponsePayload
            {
                Devices = distinctRequestDeviceIds
                    .ToDictionary(
                        queryDeviceId => queryDeviceId,
                        queryDeviceId =>
                        {
                            var device = devices.FirstOrDefault(x => x.Id == queryDeviceId);
                            var results = device is not null
                                ? device.GetGoogleState(_stateCache)
                                : offlineDeviceState;

                            // Add explicit status if not specified by state mappings
                            if (!results.ContainsKey("status"))
                            {
                                results.Add("status", "SUCCESS");
                            }

                            return results;
                        })
            };

            // Trigger reportState before returning
            var reportStateDevices = devices.Where(device => device.WillReportState).ToList();
            var shouldTriggerReportState = false;
            if (reportStateDevices.Any() && shouldTriggerReportState)
            {
                _messageHub.Publish(new ReportStateEvent { Devices = reportStateDevices });
            }

            return queryResponsePayload;
        }
    }
}
