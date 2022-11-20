using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Hosting;

namespace HomeAutio.Mqtt.GoogleHome.Services
{
    /// <summary>
    /// Google HomeGraph API service.
    /// </summary>
    public class GoogleHomeGraphService : IHostedService
    {
        private readonly IMessageHub _messageHub;
        private readonly StateCache _stateCache;
        private readonly GoogleHomeGraphClient _googleHomeGraphClient;

        private readonly IList<Guid> _messageHubSubscriptions = new List<Guid>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeGraphService"/> class.
        /// </summary>
        /// <param name="messageHub">Message hub.</param>
        /// <param name="googleHomeGraphClient">Google Home Graph API client.</param>
        /// <param name="stateCache">State cache,</param>
        public GoogleHomeGraphService(
            IMessageHub messageHub,
            GoogleHomeGraphClient googleHomeGraphClient,
            StateCache stateCache)
        {
            _messageHub = messageHub ?? throw new ArgumentNullException(nameof(messageHub));
            _googleHomeGraphClient = googleHomeGraphClient ?? throw new ArgumentNullException(nameof(googleHomeGraphClient));
            _stateCache = stateCache ?? throw new ArgumentNullException(nameof(stateCache));
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Subscribe to event aggregator
            _messageHubSubscriptions.Add(_messageHub.Subscribe<RequestSyncEvent>(HandleGoogleRequestSync));
            _messageHubSubscriptions.Add(_messageHub.Subscribe<ReportStateEvent>(HandleGoogleReportState));

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
            if (reportStateEvent.Devices.Count > 0)
            {
                await _googleHomeGraphClient.SendUpdatesAsync(reportStateEvent.Devices, _stateCache);
            }
        }
    }
}
