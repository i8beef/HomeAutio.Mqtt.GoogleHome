﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.Core;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Services
{
    /// <summary>
    /// MQTT Service.
    /// </summary>
    public class MqttService : ServiceBase
    {
        private readonly ILogger<MqttService> _log;

        private readonly IMessageHub _messageHub;
        private readonly IGoogleDeviceRepository _deviceRepository;
        private readonly StateCache _stateCache;

        private readonly IList<Guid> _messageHubSubscriptions = new List<Guid>();

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttService"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message hub.</param>
        /// <param name="brokerSettings">MQTT broker settings.</param>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="stateCache">State cache.</param>
        /// <param name="topicRoot">Topic root.</param>
        public MqttService(
            ILogger<MqttService> logger,
            IMessageHub messageHub,
            BrokerSettings brokerSettings,
            IGoogleDeviceRepository deviceRepository,
            StateCache stateCache,
            string topicRoot = "google/home")
            : base(logger, brokerSettings, topicRoot)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentNullException(nameof(messageHub));
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _stateCache = stateCache ?? throw new ArgumentNullException(nameof(stateCache));

            // Subscribe to google home based topics
            SubscribedTopics.Add(TopicRoot + "/commands/+/set");

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
            _messageHubSubscriptions.Add(_messageHub.Subscribe<ConfigSubscriptionChangeEvent>((e) => HandleConfigSubscriptionChange(e)));
            _messageHubSubscriptions.Add(_messageHub.Subscribe<DeviceCommandExecutionEvent>((e) => HandleGoogleHomeCommand(e)));
            _messageHubSubscriptions.Add(_messageHub.Subscribe<SyncIntentReceivedEvent>((e) => HandleGoogleHomeSyncIntent(e)));
            _messageHubSubscriptions.Add(_messageHub.Subscribe<QueryIntentReceivedEvent>((e) => HandleGoogleHomeQueryIntent(e)));

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
        protected override void Mqtt_MqttMsgPublishReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var message = e.ApplicationMessage.ConvertPayloadToString();
            _log.LogInformation("MQTT message received for topic {Topic}: {Message}", topic, message);

            if (topic == $"{TopicRoot}/commands/REQUEST_SYNC/set")
            {
                _messageHub.Publish(new RequestSyncEvent());
            }
            else if (_stateCache.TryGetValue(topic, out string currentState))
            {
                if (_stateCache.TryUpdate(topic, message, currentState))
                {
                    // Identify updated devices that handle reportState
                    var devices = _deviceRepository.GetAll()
                        .Where(device => !device.Disabled)
                        .Where(device => device.WillReportState)
                        .Where(device => device.Traits.Any(trait => trait.State != null && trait.State.Values.Any(state => state.Topic == topic)))
                        .ToList();

                    // Trigger reportState
                    _messageHub.Publish(new ReportStateEvent { Devices = devices });
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
            foreach (var topic in changeEvent.DeletedSubscriptions.Distinct())
            {
                // Check if actually subscribed and remove MQTT subscription
                if (SubscribedTopics.Contains(topic))
                {
                    _log.LogInformation("MQTT unsubscribing to the following topic: {Topic}", topic);
                    await MqttClient.UnsubscribeAsync(new List<string> { topic });
                    SubscribedTopics.Remove(topic);
                }

                // Check that state cache actually contains topic
                if (_stateCache.ContainsKey(topic))
                {
                    if (_stateCache.TryRemove(topic, out string _))
                        _log.LogInformation("Successfully removed topic {Topic} from internal state cache", topic);
                    else
                        _log.LogWarning("Failed to remove topic {Topic} from internal state cache", topic);
                }
            }

            // Begin listening to added topics
            foreach (var topic in changeEvent.AddedSubscriptions.Distinct())
            {
                // Ensure the that state cache doesn't contain topic and add
                if (!_stateCache.ContainsKey(topic))
                {
                    if (_stateCache.TryAdd(topic, string.Empty))
                        _log.LogInformation("Successfully added topic {Topic} to internal state cache", topic);
                    else
                        _log.LogWarning("Failed to add topic {Topic} to internal state cache", topic);
                }

                // Check if already subscribed and subscribe to MQTT topic
                if (!SubscribedTopics.Contains(topic))
                {
                    _log.LogInformation("MQTT subscribing to the following topic: {Topic}", topic);
                    await MqttClient.SubscribeAsync(
                        new List<MqttTopicFilter>
                        {
                            new MqttTopicFilterBuilder()
                                .WithTopic(topic)
                                .WithAtLeastOnceQoS()
                                .Build()
                        });
                    SubscribedTopics.Add(topic);
                }
            }
        }

        /// <summary>
        /// Handler for Google Home commands.
        /// </summary>
        /// <param name="deviceCommandExecutionEvent">The device command to handle.</param>
        private async void HandleGoogleHomeCommand(DeviceCommandExecutionEvent deviceCommandExecutionEvent)
        {
            var device = _deviceRepository.Get(deviceCommandExecutionEvent.DeviceId);
            if (device.Disabled)
                return;

            // Find all supported commands for the device
            var deviceSupportedCommands = device.Traits
                .SelectMany(x => x.Commands)
                .ToDictionary(x => x.Key, x => x.Value);

            // Check if device supports the requested command class
            var execution = deviceCommandExecutionEvent.Execution;
            if (deviceSupportedCommands.ContainsKey(execution.Command))
            {
                // Handle command delegation
                var shortCommandName = execution.Command.Substring(execution.Command.LastIndexOf('.') + 1);
                var deviceTopicName = Regex.Replace(deviceCommandExecutionEvent.DeviceId, @"\s", string.Empty);
                var delegateTopic = $"{TopicRoot}/execution/{deviceTopicName}/{shortCommandName}";
                var delegatePayload = execution.Params != null ? JsonConvert.SerializeObject(execution.Params) : "{}";

                await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithTopic(delegateTopic)
                    .WithPayload(delegatePayload)
                    .WithAtLeastOnceQoS()
                    .Build())
                    .ConfigureAwait(false);

                // Find the specific commands supported parameters it can handle
                var deviceSupportedCommandParams = deviceSupportedCommands[execution.Command] ?? new Dictionary<string, string>();

                // Handle remaining command state param negotiation
                if (execution.Params != null)
                {
                    // TODO: Remove the Where filter here eventually
                    // Flatten the parameters, ignore old delegate underscores
                    var flattenedParams = execution.Params
                        .Where(x => x.Key != "_")
                        .ToDictionary(x => x.Key, x => x.Value)
                        .ToFlatDictionary();

                    foreach (var parameter in flattenedParams)
                    {
                        // Check if device supports the requested parameter
                        if (deviceSupportedCommandParams.ContainsKey(parameter.Key))
                        {
                            // Handle remapping of command param to state key
                            var stateKey = CommandToStateKeyMapper.Map(parameter.Key);

                            // Find the DeviceState object that provides configuration for mapping state/command values
                            var deviceState = device.Traits
                                .Where(x => x.Commands.ContainsKey(execution.Command))
                                .Where(x => x.State != null)
                                .SelectMany(x => x.State)
                                .Where(x => x.Key == stateKey)
                                .Select(x => x.Value)
                                .FirstOrDefault();

                            // Build the MQTT message
                            var topic = deviceSupportedCommandParams[parameter.Key];
                            if (!string.IsNullOrEmpty(topic))
                            {
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

        /// <summary>
        /// Handler for Google Home SYNC intent.
        /// </summary>
        /// <param name="syncIntentReceivedEvent">The SYNC intent to handle.</param>
        private async void HandleGoogleHomeSyncIntent(SyncIntentReceivedEvent syncIntentReceivedEvent)
        {
            var delegateTopic = $"{TopicRoot}/sync/lastRequest";
            var delegatePayload = syncIntentReceivedEvent.Time.ToString();

            await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(delegateTopic)
                .WithPayload(delegatePayload)
                .WithAtLeastOnceQoS()
                .Build())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handler for Google Home QUERY intent.
        /// </summary>
        /// <param name="queryIntentReceivedEvent">The QUERY intent to handle.</param>
        private async void HandleGoogleHomeQueryIntent(QueryIntentReceivedEvent queryIntentReceivedEvent)
        {
            var delegateTopic = $"{TopicRoot}/query/lastRequest";
            var delegatePayload = JsonConvert.SerializeObject(queryIntentReceivedEvent);

            await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(delegateTopic)
                .WithPayload(delegatePayload)
                .WithAtLeastOnceQoS()
                .Build())
                .ConfigureAwait(false);
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
