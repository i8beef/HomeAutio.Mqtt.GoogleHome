using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Validation;
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
        public async Task<Models.Response.ExecutionResponsePayload> Handle(Models.Request.ExecuteIntent intent)
        {
            _log.LogInformation(string.Format(
                "Received EXECUTE intent for commands: {0}",
                string.Join(",", intent.Payload.Commands
                    .SelectMany(x => x.Execution)
                    .Select(x => x.Command))));

            var executionResponsePayload = new Models.Response.ExecutionResponsePayload();
            foreach (var command in intent.Payload.Commands)
            {
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
                    // Validate challenges
                    var challengeResult = ValidateChallenges(command, execution);
                    if (challengeResult != null)
                    {
                        commandResponse.Status = Models.Response.CommandStatus.Error;

                        if (execution.Challenge != null)
                        {
                            // Challenge failed
                            commandResponse.ErrorCode = challengeResult;
                            commandResponse.ChallengeNeeded = new Models.Response.ChallengeResponse
                            {
                                Type = challengeResult
                            };
                        }
                        else
                        {
                            // Challenge required
                            commandResponse.ErrorCode = "challengeNeeded";
                            commandResponse.ChallengeNeeded = new Models.Response.ChallengeResponse
                            {
                                Type = challengeResult
                            };
                        }

                        break;
                    }

                    // Get command schema and check if it has a specified result structure
                    var schemas = await TraitSchemaProvider.GetTraitSchemas();
                    var commandSchema = schemas
                        .SelectMany(x => x.CommandSchemas.Where(y => y.Command.ToEnumString() == execution.Command))
                        .FirstOrDefault();

                    if (commandSchema.ResultsJson != null)
                    {
                        // Specific result structure expected
                        // Note this path is only supported on a single device here
                        if (command.Devices.Count() == 1)
                        {
                            var trait = _deviceRepository
                                .Get(command.Devices[0].Id)
                                .Traits
                                .FirstOrDefault(x => x.Commands.ContainsKey(execution.Command));

                            if (trait != null)
                            {
                                // Note: At some point might be a good idea to scan results and only populate those
                                foreach (var state in trait.State)
                                {
                                    states.Add(state.Key, state.Value.MapValueToGoogle(null));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Generic device state response expected
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

                if (commandResponse.Status == Models.Response.CommandStatus.Success)
                {
                    // Only add any processed states if there were no challenge or validation errors
                    commandResponse.States = states;

                    // Convert command to a event to publish now that its passed all verifications
                    _messageHub.Publish(command);
                }

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
        private string ValidateChallenges(Command command, Execution execution)
        {
            // If any active devices require a challenge for this command execution, evaluate if its fulfilled by the request
            foreach (var commandDevice in command.Devices)
            {
                var device = _deviceRepository.Get(commandDevice.Id);
                if (device != null)
                {
                    if (device.Disabled)
                        continue;

                    // Get any challenges
                    var challenges = device.Traits
                        .Where(x => x.Commands.ContainsKey(execution.Command) && x.Challenge != null)
                        .Select(x => x.Challenge);

                    // Evaluate all challenges
                    foreach (var challenge in challenges)
                    {
                        // Missing challenge answer
                        if (execution.Challenge == null)
                        {
                            return challenge.ChallengeNeededPhrase;
                        }

                        // Validate challenge answer
                        if (!challenge.Validate(execution.Challenge))
                        {
                            // Challenge rejected
                            return challenge.ChallengeRejectedPhrase;
                        }
                    }
                }
            }

            // All challenges passed
            return null;
        }
    }
}
