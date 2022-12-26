using System;
using System.Collections.Generic;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Models.State;
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
        private readonly IDictionary<string, string?> _stateCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteIntentHandler"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="stateCache">State cache,</param>
        public ExecuteIntentHandler(
            ILogger<ExecuteIntentHandler> logger,
            IMessageHub messageHub,
            IGoogleDeviceRepository deviceRepository,
            StateCache stateCache)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentNullException(nameof(messageHub));
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _stateCache = stateCache ?? throw new ArgumentNullException(nameof(stateCache));
        }

        /// <summary>
        /// Handles a <see cref="ExecuteIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.ExecutionResponsePayload"/>.</returns>
        public Models.Response.ExecutionResponsePayload Handle(ExecuteIntent intent)
        {
            _log.LogInformation(string.Format(
                "Received EXECUTE intent for commands: {0}",
                string.Join(",", intent.Payload.Commands
                    .SelectMany(x => x.Execution)
                    .Select(x => x.Command))));

            // Get all device ids from commands to split into per-device responses
            var deviceIds = intent.Payload.Commands
                .SelectMany(x => x.Devices.Select(y => y.Id))
                .Distinct();

            var responseCommands = new List<Models.Response.Command>();
            foreach (var deviceId in deviceIds)
            {
                var device = _deviceRepository.FindById(deviceId);
                if (device == null)
                {
                    // Device not found
                    responseCommands.Add(new Models.Response.Command
                    {
                        Ids = new List<string> { deviceId },
                        ErrorCode = "deviceNotFound",
                        Status = Models.Response.CommandStatus.Error
                    });

                    continue;
                }

                var commands = intent.Payload.Commands.Where(x => x.Devices.Any(y => y.Id == deviceId));
                var executions = commands.SelectMany(x => x.Execution);

                // Validate all device challenge checks
                var challengesPassed = true;
                foreach (var execution in executions)
                {
                    var challengeResult = ValidateChallenges(device, execution);
                    if (challengeResult != null)
                    {
                        challengesPassed = false;
                        if (execution.Challenge != null)
                        {
                            // Challenge failed
                            responseCommands.Add(new Models.Response.Command
                            {
                                Ids = new List<string> { deviceId },
                                ErrorCode = challengeResult,
                                Status = Models.Response.CommandStatus.Error,
                                ChallengeNeeded = new Models.Response.ChallengeResponse
                                {
                                    Type = challengeResult
                                }
                            });
                        }
                        else
                        {
                            // Challenge required
                            responseCommands.Add(new Models.Response.Command
                            {
                                Ids = new List<string> { deviceId },
                                ErrorCode = "challengeNeeded",
                                Status = Models.Response.CommandStatus.Error,
                                ChallengeNeeded = new Models.Response.ChallengeResponse
                                {
                                    Type = challengeResult
                                }
                            });
                        }

                        break;
                    }
                }

                // Challenge missing or failed
                if (!challengesPassed)
                {
                    continue;
                }

                // Publish command and build state response
                IDictionary<string, object?>? responseStates = null;
                var schemas = TraitSchemaProvider.GetTraitSchemas();
                foreach (var command in commands)
                {
                    foreach (var execution in command.Execution)
                    {
                        // Convert command to an event to publish now that its passed all verifications
                        var commandEvent = new DeviceCommandExecutionEvent { DeviceId = deviceId, Execution = execution };
                        _messageHub.Publish(commandEvent);

                        // Generate state for response
                        var states = new Dictionary<string, object?>();
                        var trait = device.Traits.FirstOrDefault(x => x.Commands.ContainsKey(execution.Command));
                        if (trait != null)
                        {
                            var traitSchema = schemas.First(x => x.Trait == trait.Trait);
                            var commandSchema = traitSchema.CommandSchemas.First(x => x.Command == execution.Command.ToEnum<CommandType>());

                            var googleState = trait.GetGoogleStateFlattened(_stateCache, traitSchema);

                            // Map incoming params to "fake" state changes to override existing state value
                            var replacedParams = execution.Params != null
                                ? execution.Params.ToFlatDictionary().ToDictionary(kvp => CommandToStateKeyMapper.Map(kvp.Key), kvp => kvp.Value)
                                : new Dictionary<string, object?>();

                            foreach (var state in googleState)
                            {
                                // Decide to use existing state cache value, or attempt to take from transformed execution params
                                var value = replacedParams.TryGetValue(state.Key, out var stateValue) ? stateValue : state.Value;

                                // Only add to state response if specified in the command result schema, or fallback state schema
                                if (commandSchema.ResultsValidator != null)
                                {
                                    if (commandSchema.ResultsValidator.FlattenedPathExists(state.Key))
                                    {
                                        states.Add(state.Key, value);
                                    }
                                }
                                else if (traitSchema.StateSchema != null)
                                {
                                    if (traitSchema.StateSchema.Validator.FlattenedPathExists(state.Key))
                                    {
                                        states.Add(state.Key, value);
                                    }
                                }
                            }
                        }

                        // Add explicit online if not specified by state mappings
                        if (!states.ContainsKey("online"))
                        {
                            states.Add("online", true);
                        }

                        // Add any processed states
                        responseStates = states.ToNestedDictionary();
                    }
                }

                // Prepare device response payload
                var deviceCommandResponse = new Models.Response.Command
                {
                    Ids = new List<string> { deviceId },
                    Status = Models.Response.CommandStatus.Success,
                    States = responseStates
                };

                responseCommands.Add(deviceCommandResponse);
            }

            var executionResponsePayload = new Models.Response.ExecutionResponsePayload
            {
                Commands = responseCommands
            };

            return executionResponsePayload;
        }

        /// <summary>
        /// Checks if the command is a delegated command for the given device.
        /// </summary>
        /// <param name="device">Device to check.</param>
        /// <param name="execution">Command execution to check.</param>
        /// <returns><c>true</c> if command is handled as a delegated command, otherwise <c>false</c>.</returns>
        private static string? ValidateChallenges(Models.State.Device device, Execution execution)
        {
            if (device != null && !device.Disabled)
            {
                // Get any challenges
                var challenges = device.Traits
                    .Where(x => x.Commands.ContainsKey(execution.Command) && x.Challenge != null)
                    .Select(x => x.Challenge!);

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

            // All challenges passed
            return null;
        }
    }
}
