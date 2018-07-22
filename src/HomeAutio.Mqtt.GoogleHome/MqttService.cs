using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.Core;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Hosting;
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

        private readonly DeviceConfiguration _deviceConfig;
        private readonly StateCache _stateCache;
        private readonly IMessageHub _messageHub;
        private readonly IList<Guid> _messageHubSubscriptions = new List<Guid>();
        private readonly GoogleHomeGraphClient _googleHomeGraphClient;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttService"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="deviceConfiguration">Ddevice configuration.</param>
        /// <param name="stateCache">State cache,</param>
        /// <param name="messageHub">Message hub.</param>
        /// <param name="googleHomeGraphClient">Google Home Graph API client.</param>
        /// <param name="brokerSettings">MQTT broker settings.</param>
        public MqttService(
            ILogger<MqttService> logger,
            DeviceConfiguration deviceConfiguration,
            StateCache stateCache,
            IMessageHub messageHub,
            GoogleHomeGraphClient googleHomeGraphClient,
            BrokerSettings brokerSettings)
            : base(logger, brokerSettings, "google/home/")
        {
            _log = logger;
            _deviceConfig = deviceConfiguration;
            _stateCache = stateCache;
            _messageHub = messageHub;
            _googleHomeGraphClient = googleHomeGraphClient;

            // Subscribe to google home based topics
            SubscribedTopics.Add(TopicRoot + "#");

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
            if (_stateCache.ContainsKey(e.ApplicationMessage.Topic))
            {
                _stateCache[e.ApplicationMessage.Topic] = e.ApplicationMessage.ConvertPayloadToString();

                // Handle reportState
                if (_googleHomeGraphClient != null)
                {
                    // Identify devices that handle reportState
                    var devices = _deviceConfig.Values
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
        }

        #region Google Home Handlers

        /// <summary>
        /// Hanlder for Google Home commands.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        private async void HandleGoogleHomeCommand(Command command)
        {
            foreach (var device in command.Devices)
            {
                // Find all supported commands for the device
                var deviceSupportedCommands = _deviceConfig[device.Id].Traits
                    .SelectMany(x => x.Commands)
                    .ToDictionary(x => x.Key, x => x.Value);

                foreach (var execution in command.Execution)
                {
                    // Check if device supports the requested command class
                    if (deviceSupportedCommands.ContainsKey(execution.Command))
                    {
                        // Find the specific commands supported parameters it can handle
                        var deviceSupportedParams = deviceSupportedCommands[execution.Command];

                        // Flatten the parameters
                        var flattenedParams = new Dictionary<string, object>();
                        foreach (var param in execution.Params)
                        {
                            var paramValueAsDictionary = param.Value as IDictionary<string, object>;
                            if (paramValueAsDictionary != null)
                            {
                                // Add each of the sub params as a flattened, prefixed parameter
                                foreach (var subParam in paramValueAsDictionary)
                                {
                                    flattenedParams.Add(param.Key + '.' + subParam.Key, subParam.Value);
                                }
                            }
                            else
                            {
                                // Pipe through original value
                                flattenedParams.Add(param.Key, param.Value);
                            }
                        }

                        foreach (var parameter in flattenedParams)
                        {
                            // Check if device supports the requested parameter
                            if (deviceSupportedParams.ContainsKey(parameter.Key))
                            {
                                var deviceState = _deviceConfig[device.Id].Traits
                                    .Where(x => x.Commands.ContainsKey(execution.Command))
                                    .SelectMany(x => x.State)
                                    .Where(x => x.Key == parameter.Key)
                                    .Select(x => x.Value)
                                    .FirstOrDefault();

                                // Send the MQTT message
                                var topic = deviceSupportedParams[parameter.Key];
                                var payload = deviceState.MapValueToMqtt(parameter.Key, parameter.Value);
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
