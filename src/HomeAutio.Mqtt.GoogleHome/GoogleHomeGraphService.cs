using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Google HomeGraph API service.
    /// </summary>
    public class GoogleHomeGraphService : IHostedService
    {
        private readonly ILogger<GoogleHomeGraphService> _log;

        private readonly IMessageHub _messageHub;
        private readonly IGoogleDeviceRepository _deviceRepository;
        private readonly StateCache _stateCache;
        private readonly GoogleHomeGraphClient _googleHomeGraphClient;

        private readonly IList<Guid> _messageHubSubscriptions = new List<Guid>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeGraphService"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message hub.</param>
        /// <param name="googleHomeGraphClient">Google Home Graph API client.</param>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="stateCache">State cache,</param>
        public GoogleHomeGraphService(
            ILogger<GoogleHomeGraphService> logger,
            IMessageHub messageHub,
            GoogleHomeGraphClient googleHomeGraphClient,
            IGoogleDeviceRepository deviceRepository,
            StateCache stateCache)
        {
            _log = logger ?? throw new ArgumentException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentException(nameof(messageHub));
            _googleHomeGraphClient = googleHomeGraphClient ?? throw new ArgumentException(nameof(googleHomeGraphClient));
            _deviceRepository = deviceRepository ?? throw new ArgumentException(nameof(deviceRepository));
            _stateCache = stateCache ?? throw new ArgumentException(nameof(stateCache));
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Subscribe to event aggregator
            _messageHubSubscriptions.Add(_messageHub.Subscribe<RequestSyncEvent>((e) => HandleGoogleRequestSync(e)));
            _messageHubSubscriptions.Add(_messageHub.Subscribe<ReportStateEvent>((e) => HandleGoogleReportState(e)));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Unsubscribe all message hub subscriptions
            foreach (var token in _messageHubSubscriptions)
            {
                _messageHub.Unsubscribe(token);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// REQUEST_SYNC event handler.
        /// </summary>
        /// <param name="requestSyncEvent">Request sync event trigger.</param>
        private async void HandleGoogleRequestSync(RequestSyncEvent requestSyncEvent)
        {
            await _googleHomeGraphClient.RequestSyncAsync();
        }

        /// <summary>
        /// ReportStateAndNotification event handler.
        /// </summary>
        /// <param name="reportStateEvent">Report state event to handle.</param>
        private async void HandleGoogleReportState(ReportStateEvent reportStateEvent)
        {
            // Send updated to Google Home Graph
            if (reportStateEvent.Devices.Count() > 0)
            {
                await _googleHomeGraphClient.SendUpdatesAsync(reportStateEvent.Devices, _stateCache);
            }
        }
    }
}
