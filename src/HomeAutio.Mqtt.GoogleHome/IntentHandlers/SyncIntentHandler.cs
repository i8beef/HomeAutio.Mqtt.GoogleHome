using System;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.IntentHandlers
{
    /// <summary>
    /// Sync intent handler.
    /// </summary>
    public class SyncIntentHandler
    {
        private readonly ILogger<SyncIntentHandler> _log;

        private readonly IMessageHub _messageHub;
        private readonly IConfiguration _config;
        private readonly IGoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="deviceRepository">Device repository.</param>
        public SyncIntentHandler(
            ILogger<SyncIntentHandler> logger,
            IMessageHub messageHub,
            IConfiguration configuration,
            IGoogleDeviceRepository deviceRepository)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentNullException(nameof(messageHub));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.SyncIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.SyncResponsePayload"/>.</returns>
        public Models.Response.SyncResponsePayload Handle(Models.Request.SyncIntent intent)
        {
            _log.LogInformation("Received SYNC intent");

            // Convert to an event to publish
            var commandEvent = new SyncIntentReceivedEvent { Time = DateTimeOffset.Now };
            _messageHub.Publish(commandEvent);

            var syncResponsePayload = new Models.Response.SyncResponsePayload
            {
                AgentUserId = _config.GetRequiredValue<string>("googleHomeGraph:agentUserId"),
                Devices = _deviceRepository.GetAll()
                    .Where(device => !device.Disabled)
                    .Select(x => new Models.Response.Device
                    {
                        Id = x.Id,
                        Type = x.Type,
                        RoomHint = x.RoomHint,
                        WillReportState = x.WillReportState,
                        Traits = x.Traits.Select(trait => trait.Trait).ToList(),
                        Attributes = x.Traits
                            .Where(trait => trait.Attributes != null)
                            .SelectMany(trait => trait.Attributes!)
                            .ToDictionary(kv => kv.Key, kv => kv.Value),
                        Name = x.Name,
                        DeviceInfo = x.DeviceInfo,
                        CustomData = x.CustomData
                    }).ToList()
            };

            return syncResponsePayload;
        }
    }
}
