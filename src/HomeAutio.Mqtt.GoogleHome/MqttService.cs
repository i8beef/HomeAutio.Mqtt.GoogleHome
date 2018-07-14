using Easy.MessageHub;
using HomeAutio.Mqtt.Core;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAutio.Mqtt.GoogleHome
{
    public class MqttService : ServiceBase
    {
        private ILogger<MqttService> _log;
        private bool _disposed = false;

        private readonly DeviceConfiguration _deviceConfig;
        private readonly StateCache _stateCache;
        private readonly IMessageHub _messageHub;
        private readonly IList<Guid> _messageHubSubscriptions = new List<Guid>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationLifetime"></param>
        /// <param name="logger"></param>
        /// <param name="deviceConfiguration"></param>
        /// <param name="stateCache"></param>
        /// <param name="messageHub"></param>
        /// <param name="brokerIp"></param>
        /// <param name="brokerPort"></param>
        /// <param name="brokerUsername"></param>
        /// <param name="brokerPassword"></param>
        public MqttService(
            IApplicationLifetime applicationLifetime,
            ILogger<MqttService> logger,
            DeviceConfiguration deviceConfiguration,
            StateCache stateCache,
            IMessageHub messageHub,
            string brokerIp,
            int brokerPort = 1883,
            string brokerUsername = null,
            string brokerPassword = null)
            : base(applicationLifetime, logger, brokerIp, brokerPort, brokerUsername, brokerPassword, "google/home/")
        {
            _log = logger;
            _deviceConfig = deviceConfiguration;
            _stateCache = stateCache;
            _messageHub = messageHub;

            // Subscribe to google home based topics
            SubscribedTopics.Add(TopicRoot + "#");

            // Subscribe to all monitored state topics
            foreach (var topic in _stateCache.Keys)
            {
                SubscribedTopics.Add(topic);
            }
        }

        protected override Task StartServiceAsync(CancellationToken cancellationToken)
        {
            // Subscribe to event aggregator
            _messageHubSubscriptions.Add(_messageHub.Subscribe<Command>((e) => HandleGoogleHomeCommand(e)));

            return Task.CompletedTask;
        }

        protected override Task StopServiceAsync(CancellationToken cancellationToken)
        {
            // Unsubscribe all message hub subscriptions
            foreach (var token in _messageHubSubscriptions)
            {
                _messageHub.Unsubscribe(token);
            }

            return Task.CompletedTask;
        }

        protected override void Mqtt_MqttMsgPublishReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (_stateCache.ContainsKey(e.ApplicationMessage.Topic))
            {
                _stateCache[e.ApplicationMessage.Topic] = e.ApplicationMessage.ConvertPayloadToString();
            }

            // TODO: If report state, report here
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
                                var payload = MapValue(deviceState, parameter.Key, parameter.Value);
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

        /// <summary>
        /// Handles mapping some common state values to google acceptable state values.
        /// </summary>
        /// <param name="paramKey">Param key.</param>
        /// <param name="stateValue">State value.</param>
        /// <returns>Remapped value.</returns>
        private string MapValue(DeviceState deviceState, string paramKey, object stateValue)
        {
            // Default to string version of passed parameter value
            var mappedValue = stateValue.ToString();

            if (deviceState.ValueMap != null && deviceState.ValueMap.Count > 0)
            {
                foreach (var valueMap in deviceState.ValueMap)
                {
                    if (valueMap.MatchesGoogle(stateValue))
                    {
                        // Do value comparison, break on first match
                        mappedValue = valueMap.ConvertToMqtt(stateValue);
                        break;
                    }
                }
            }

            return mappedValue;
        }

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
