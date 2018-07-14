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
                    if (deviceSupportedCommands.ContainsKey(execution.Command))
                    {
                        // Find the specific commands supported parameters it can handle
                        var deviceSupportedParams = deviceSupportedCommands[execution.Command];
                        foreach (var parameter in execution.Params)
                        {
                            if (deviceSupportedParams.ContainsKey(parameter.Key))
                            {
                                switch (parameter.Value)
                                {
                                    case IDictionary<string, object> dictionaryObject:
                                        foreach (var subParam in dictionaryObject)
                                        {
                                            if (((IDictionary<string, object>)deviceSupportedParams[parameter.Key]).ContainsKey(subParam.Key))
                                            {
                                                // Send command
                                                var subTopic = ((IDictionary<string, object>)deviceSupportedParams[parameter.Key])[subParam.Key] as string;
                                                var subPayload = subParam.Value.ToString();
                                                await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                                    .WithTopic(subTopic)
                                                    .WithPayload(subPayload)
                                                    .WithAtLeastOnceQoS()
                                                    .Build())
                                                    .ConfigureAwait(false);
                                            }
                                        }
                                        break;
                                    default:
                                        // Send command
                                        var topic = deviceSupportedParams[parameter.Key] as string;
                                        var payload = parameter.Value.ToString();
                                        await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                            .WithTopic(topic)
                                            .WithPayload(payload)
                                            .WithAtLeastOnceQoS()
                                            .Build())
                                            .ConfigureAwait(false);
                                        break;
                                }
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
