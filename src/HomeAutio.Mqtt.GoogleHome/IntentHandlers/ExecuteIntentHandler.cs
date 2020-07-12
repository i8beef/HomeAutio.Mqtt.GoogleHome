using System;
using System.Collections.Generic;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
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
        private readonly IGoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="deviceRepository">Device repository.</param>
        public ExecuteIntentHandler(
            ILogger<ExecuteIntentHandler> logger,
            IMessageHub messageHub,
            IGoogleDeviceRepository deviceRepository)
        {
            _log = logger ?? throw new ArgumentException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentException(nameof(messageHub));
            _deviceRepository = deviceRepository ?? throw new ArgumentException(nameof(deviceRepository));
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
                    // Dont bother with states for command delegation
                    if (IsDelegatedCommand(command, execution))
                    {
                        break;
                    }

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
                            .ToDictionary(kvp => CommandToStateKeyMapper.Map(kvp.Key), kvp => kvp.Value)
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
        /// Checks if the command is a delegated command for the given device.
        /// </summary>
        /// <param name="command">Command to check.</param>
        /// <param name="execution">Command execution to check.</param>
        /// <returns><c>true</c> if command is handled as a delegated command, otherwise <c>false</c>.</returns>
        private bool IsDelegatedCommand(Command command, Execution execution)
        {
            var isDelegatedCommand = false;
            foreach (var commandDevice in command.Devices)
            {
                var device = _deviceRepository.Get(commandDevice.Id);
                if (device.Disabled)
                    continue;

                // Find all supported commands for the device
                var deviceSupportedCommands = device.Traits
                    .SelectMany(x => x.Commands)
                    .ToDictionary(x => x.Key, x => x.Value);

                // Check if device supports the requested command class
                if (deviceSupportedCommands.ContainsKey(execution.Command))
                {
                    // Find the specific commands supported parameters it can handle
                    var deviceSupportedCommandParams = deviceSupportedCommands[execution.Command];

                    // Handle command delegation
                    if (deviceSupportedCommandParams.ContainsKey("_"))
                    {
                        isDelegatedCommand = true;
                        break;
                    }
                }
            }

            return isDelegatedCommand;
        }
    }
}
