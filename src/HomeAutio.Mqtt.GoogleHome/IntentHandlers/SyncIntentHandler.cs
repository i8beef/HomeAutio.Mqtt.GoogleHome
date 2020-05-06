using System;
using System.Linq;
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

        private readonly IConfiguration _config;
        private readonly IGoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="deviceRepository">Device repository.</param>
        public SyncIntentHandler(
            ILogger<SyncIntentHandler> logger,
            IConfiguration configuration,
            IGoogleDeviceRepository deviceRepository)
        {
            _log = logger ?? throw new ArgumentException(nameof(logger));
            _config = configuration ?? throw new ArgumentException(nameof(configuration));
            _deviceRepository = deviceRepository ?? throw new ArgumentException(nameof(deviceRepository));
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.SyncIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.SyncResponsePayload"/>.</returns>
        public Models.Response.SyncResponsePayload Handle(Models.Request.SyncIntent intent)
        {
            _log.LogInformation("Received SYNC intent");

            var syncResponsePayload = new Models.Response.SyncResponsePayload
            {
                AgentUserId = _config.GetValue<string>("googleHomeGraph:agentUserId"),
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
                            .SelectMany(trait => trait.Attributes)
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
