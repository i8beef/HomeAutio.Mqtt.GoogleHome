using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.Core;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// MQTT Service.
    /// </summary>
    public class MqttService : ServiceBase
    {
        private readonly ILogger<MqttService> _log;

        private readonly GoogleDeviceRepository _deviceRepository;
        private readonly StateCache _stateCache;
        private readonly IMessageHub _messageHub;
        private readonly IList<Guid> _messageHubSubscriptions = new List<Guid>();
        private readonly GoogleHomeGraphClient _googleHomeGraphClient;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttService"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="stateCache">State cache,</param>
        /// <param name="messageHub">Message hub.</param>
        /// <param name="googleHomeGraphClient">Google Home Graph API client.</param>
        /// <param name="brokerSettings">MQTT broker settings.</param>
        public MqttService(
            ILogger<MqttService> logger,
            GoogleDeviceRepository deviceRepository,
            StateCache stateCache,
            IMessageHub messageHub,
            GoogleHomeGraphClient googleHomeGraphClient,
            BrokerSettings brokerSettings)
            : base(logger, brokerSettings, "google/home")
        {
            _log = logger;
            _deviceRepository = deviceRepository;
            _stateCache = stateCache;
            _messageHub = messageHub;
            _googleHomeGraphClient = googleHomeGraphClient;

            // Subscribe to google home based topics
            SubscribedTopics.Add(TopicRoot + "/#");

            // Subscribe to all monitored state topics
            foreach (var topic in _stateCache.Keys)
            {
                SubscribedTopics.Add(topic);
            }
        }

        /// <inheritdoc />
        protected override Task StartServiceAsync(CancellationToken cancellationToken)
        {
            // Subscribe to event aggregator
            _messageHubSubscriptions.Add(_messageHub.Subscribe<Command>((e) => HandleGoogleHomeCommand(e)));
            _messageHubSubscriptions.Add(_messageHub.Subscribe<ConfigSubscriptionChangeEvent>((e) => HandleConfigSubscriptionChange(e)));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task StopServiceAsync(CancellationToken cancellationToken)
        {
            // Unsubscribe all message hub subscriptions
            foreach (var token in _messageHubSubscriptions)
            {
                _messageHub.Unsubscribe(token);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override void Mqtt_MqttMsgPublishReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var message = e.ApplicationMessage.ConvertPayloadToString();
            _log.LogInformation("MQTT message received for topic " + e.ApplicationMessage.Topic + ": " + message);

            if (e.ApplicationMessage.Topic == TopicRoot + "/REQUEST_SYNC")
            {
                // Handle REQUEST_SYNC
                _googleHomeGraphClient.RequestSyncAsync()
                    .GetAwaiter().GetResult();
            }
            else if (_stateCache.ContainsKey(e.ApplicationMessage.Topic))
            {
                _stateCache[e.ApplicationMessage.Topic] = message;

                // Identify devices that handle reportState
                var devices = _deviceRepository.GetAll()
                    .Where(x => x.WillReportState)
                    .Where(x => x.Traits.Any(trait => trait.State.Values.Any(state => state.Topic == e.ApplicationMessage.Topic)))
                    .ToList();

                // Send updated to Google Home Graph
                if (devices.Count() > 0)
                {
                    _googleHomeGraphClient.SendUpdatesAsync(devices, _stateCache)
                        .GetAwaiter().GetResult();
                }
            }
        }

        #region Google Home Handlers

        /// <summary>
        /// Handler for device config change events.
        /// </summary>
        /// <param name="changeEvent">The change event to handle.</param>
        private async void HandleConfigSubscriptionChange(ConfigSubscriptionChangeEvent changeEvent)
        {
            // Stop listening to removed topics
            await MqttClient.UnsubscribeAsync(changeEvent.DeletedSubscriptions);
            foreach (var topic in changeEvent.DeletedSubscriptions)
                SubscribedTopics.Remove(topic);

            // Begin listening to added topics
            await MqttClient.SubscribeAsync(changeEvent
                .AddedSubscriptions
                .Select(topic => new TopicFilterBuilder()
                    .WithTopic(topic)
                    .WithAtLeastOnceQoS()
                    .Build()));
            foreach (var topic in changeEvent.AddedSubscriptions)
                SubscribedTopics.Add(topic);
        }

        /// <summary>
        /// Hanlder for Google Home commands.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        private async void HandleGoogleHomeCommand(Command command)
        {
            foreach (var device in command.Devices)
            {
                // Find all supported commands for the device
                var deviceSupportedCommands = _deviceRepository.Get(device.Id).Traits
                    .SelectMany(x => x.Commands)
                    .ToDictionary(x => x.Key, x => x.Value);

                foreach (var execution in command.Execution)
                {
                    // Check if device supports the requested command class
                    if (deviceSupportedCommands.ContainsKey(execution.Command))
                    {
                        // Find the specific commands supported parameters it can handle
                        var deviceSupportedCommandParams = deviceSupportedCommands[execution.Command];

                        // Flatten the parameters
                        var flattenedParams = execution.Params.ToFlatDictionary();

                        foreach (var parameter in flattenedParams)
                        {
                            // Check if device supports the requested parameter
                            if (deviceSupportedCommandParams.ContainsKey(parameter.Key))
                            {
                                // Handle remapping of Modes, Toggles and FanSpeed
                                var stateKey = CommandToStateKeyMapper.Map(parameter.Key);

                                // Find the DeviceState object that provides configuration for mapping state/command values
                                var deviceState = _deviceRepository.Get(device.Id).Traits
                                    .Where(x => x.Commands.ContainsKey(execution.Command))
                                    .SelectMany(x => x.State)
                                    .Where(x => x.Key == stateKey)
                                    .Select(x => x.Value)
                                    .FirstOrDefault();

                                // Build the MQTT message
                                var topic = deviceSupportedCommandParams[parameter.Key];
                                string payload = null;
                                if (deviceState != null)
                                {
                                    payload = deviceState.MapValueToMqtt(parameter.Value);
                                }
                                else
                                {
                                    payload = parameter.Value.ToString();
                                    _log.LogWarning("Received supported command '{Command}' but cannot find matched state config, sending command value '{Payload}' without ValueMap", execution.Command, payload);
                                }

                                await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                    .WithTopic(topic)
                                    .WithPayload(payload)
                                    .WithAtLeastOnceQoS()
                                    .Build())
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Dispose implementation.
        /// </summary>
        /// <param name="disposing">Indicates if disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        #endregion
    }
}
