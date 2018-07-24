using System.Collections.Generic;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.Controllers
{
    /// <summary>
    /// Google home controller.
    /// </summary>
    [Route("/smarthome")]
    public class GoogleHomeController : Controller
    {
        private readonly ILogger<GoogleHomeController> _log;

        private readonly IConfiguration _config;
        private readonly IMessageHub _messageHub;
        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly StateCache _stateCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeController"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="deviceConfiguration">Device configuration.</param>
        /// <param name="stateCache">State cache.</param>
        public GoogleHomeController(
            ILogger<GoogleHomeController> logger,
            IConfiguration configuration,
            IMessageHub messageHub,
            DeviceConfiguration deviceConfiguration,
            StateCache stateCache)
        {
            _log = logger;
            _config = configuration;
            _messageHub = messageHub;
            _deviceConfiguration = deviceConfiguration;
            _stateCache = stateCache;
        }

        /// <summary>
        /// Post handler.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Post([FromBody]Models.Request.Request request)
        {
            // Begin building Response
            var response = new Models.Response.Response { RequestId = request.RequestId };

            // Smart Home Intents use a single object in inputs, containing the intent value, and a payload object with automation-specific objects.
            if (request.Inputs == null || request.Inputs.Count != 1)
            {
                response.Payload = new Models.Response.ErrorResponsePayload { ErrorCode = "protocolError" };
                return BadRequest(response);
            }

            var input = request.Inputs[0];
            switch (input)
            {
                case Models.Request.SyncIntent syncIntent:
                    response.Payload = HandleSyncIntent(syncIntent);
                    return Ok(response);
                case Models.Request.QueryIntent queryIntent:
                    response.Payload = HandleQueryIntent(queryIntent);
                    return Ok(response);
                case Models.Request.ExecuteIntent executeIntent:
                    response.Payload = HandleExecuteIntent(executeIntent);
                    return Ok(response);
                case Models.Request.DisconnectIntent disconnectIntent:
                    return Ok();
            }

            // No valid intents found, return error
            response.Payload = new Models.Response.ErrorResponsePayload { ErrorCode = "protocolError" };
            return BadRequest(response);
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.ExecuteIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.ExecutionResponsePayload"/>.</returns>
        private Models.Response.ExecutionResponsePayload HandleExecuteIntent(Models.Request.ExecuteIntent intent)
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
                            var trait = _deviceConfiguration[command.Devices[0].Id].Traits.FirstOrDefault();
                            if (trait != null)
                            {
                                foreach (var state in trait.State)
                                {
                                    states.Add(state.Key, state.Value.MapValueToGoogle(state.Key, null));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Copy the incoming state values, rather than getting current
                        foreach (var param in execution.Params)
                        {
                            // Handle remapping of Modes and Toggles
                            if (param.Key == "updateModeSettings")
                            {
                                states.Add("currentModeSettings", param.Value);
                            }
                            else if (param.Key == "updateToggleSettings")
                            {
                                states.Add("currentToggleSettings", param.Value);
                            }
                            else
                            {
                                states.Add(param.Key, param.Value);
                            }
                        }
                    }
                }

                commandResponse.States = states;

                executionResponsePayload.Commands.Add(commandResponse);
            }

            return executionResponsePayload;
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.QueryIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.QueryResponsePayload"/>.</returns>
        private Models.Response.QueryResponsePayload HandleQueryIntent(Models.Request.QueryIntent intent)
        {
            _log.LogInformation("Received QUERY intent for devices: " + string.Join(", ", intent.Payload.Devices.Select(x => x.Id)));

            var queryResponsePayload = new Models.Response.QueryResponsePayload
            {
                Devices = intent.Payload.Devices.ToDictionary(
                    x => x.Id,
                    x => _deviceConfiguration[x.Id].GetGoogleState(_stateCache))
            };

            return queryResponsePayload;
        }

        /// <summary>
        /// Handles a <see cref="Models.Request.SyncIntent"/>.
        /// </summary>
        /// <param name="intent">Intent to process.</param>
        /// <returns>A <see cref="Models.Response.SyncResponsePayload"/>.</returns>
        private Models.Response.SyncResponsePayload HandleSyncIntent(Models.Request.SyncIntent intent)
        {
            _log.LogInformation("Received SYNC intent");

            var syncResponsePayload = new Models.Response.SyncResponsePayload
            {
                AgentUserId = _config.GetValue<string>("googleHomeAgentUserId"),
                Devices = _deviceConfiguration.Values.Select(x => new Models.Response.Device
                {
                    Id = x.Id,
                    Type = x.Type,
                    RoomHint = x.RoomHint,
                    WillReportState = x.WillReportState,
                    Traits = x.Traits.Select(trait => trait.Trait).ToList(),
                    Attributes = x.Traits
                        .Where(trait => trait.Attributes != null)
                        .SelectMany(trait => trait.Attributes)
                        .ToDictionary(kv => kv.Key, kv => kv.Value),
                    Name = x.Name,
                    DeviceInfo = x.DeviceInfo,
                    CustomData = x.CustomData
                }).ToList()
            };

            return syncResponsePayload;
        }
    }
}
