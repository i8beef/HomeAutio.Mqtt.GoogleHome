using System.Collections.Generic;
using System.Linq;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.IntentHandlers
{
    /// <summary>
    /// Execute intent handler.
    /// </summary>
    public class ExecuteIntentHandler
    {
        private readonly ILogger<ExecuteIntentHandler> _log;

        private readonly IMessageHub _messageHub;
        private readonly GoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="deviceRepository">Device repository.</param>
        public ExecuteIntentHandler(
            ILogger<ExecuteIntentHandler> logger,
            IMessageHub messageHub,
            GoogleDeviceRepository deviceRepository)
        {
            _log = logger;
            _messageHub = messageHub;
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.ExecuteIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.ExecutionResponsePayload"/>.</returns>
        public Models.Response.ExecutionResponsePayload Handle(Models.Request.ExecuteIntent intent)
        {
            _log.LogInformation(string.Format(
                "Received EXECUTE intent for commands: {0}",
                string.Join(",", intent.Payload.Commands
                    .SelectMany(x => x.Execution)
                    .Select(x => x.Command))));

            var executionResponsePayload = new Models.Response.ExecutionResponsePayload();
            foreach (var command in intent.Payload.Commands)
            {
                // Convert command to a event to publish
                _messageHub.Publish(command);

                // Build response payload
                var commandResponse = new Models.Response.Command
                {
                    Status = Models.Response.CommandStatus.Success,
                    Ids = command.Devices.Select(x => x.Id).ToList()
                };

                // Generate states
                var states = new Dictionary<string, object>();
                foreach (var execution in command.Execution)
                {
                    // Handle camera stream commands
                    if (execution.Command == "action.devices.commands.GetCameraStream")
                    {
                        // Only allow a single cast command at once
                        if (command.Devices.Count() == 1)
                        {
                            // Get the first trait for the camera, as this should be the only trait available
                            var trait = _deviceRepository.Get(command.Devices[0].Id).Traits.FirstOrDefault();
                            if (trait != null)
                            {
                                foreach (var state in trait.State)
                                {
                                    states.Add(state.Key, state.Value.MapValueToGoogle(null));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Copy the incoming state values, rather than getting current as they won't be updated yet
                        var replacedParams = execution.Params
                            .ToFlatDictionary()
                            .ToDictionary(kvp => ConvertCommandKeyToStateKey(kvp.Key), kvp => kvp.Value)
                            .ToNestedDictionary();

                        foreach (var param in replacedParams)
                        {
                            states.Add(param.Key, param.Value);
                        }
                    }
                }

                commandResponse.States = states;

                executionResponsePayload.Commands.Add(commandResponse);
            }

            return executionResponsePayload;
        }

        /// <summary>
        /// Gets the state key for the specified command parameter key.
        /// </summary>
        /// <param name="paramKey">Command parameter key.</param>
        /// <returns>The state key associated with the command param key.</returns>
        private string ConvertCommandKeyToStateKey(string paramKey)
        {
            if (string.IsNullOrEmpty(paramKey))
                return paramKey;

            var replacements = new Dictionary<string, string>
            {
                { "updateModeSettings", "currentModeSettings" },
                { "updateToggleSettings", "currentToggleSettings" },
                { "fanSpeed", "currentFanSpeedSetting" },
                { "color.temperature", "color.temperatureK" }
            };

            foreach (var replacement in replacements)
            {
                if (paramKey.StartsWith(replacement.Key))
                {
                    return replacement.Value + paramKey.Substring(replacement.Value.Length - 1);
                }
            }

            return paramKey;
        }
    }
}
