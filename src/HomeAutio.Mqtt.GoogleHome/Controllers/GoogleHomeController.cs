using System;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAutio.Mqtt.GoogleHome.Controllers
{
    /// <summary>
    /// Google home controller.
    /// </summary>
    [Route("/smarthome")]
    public class GoogleHomeController : Controller
    {
        private readonly SyncIntentHandler _syncIntentHandler;
        private readonly QueryIntentHandler _queryIntentHandler;
        private readonly ExecuteIntentHandler _executeIntentHandler;
        private readonly DisconnectIntentHandler _disconnectIntentHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeController"/> class.
        /// </summary>
        /// <param name="syncIntentHandler">Sync intent handler.</param>
        /// <param name="queryIntentHandler">Query ntent handler.</param>
        /// <param name="executeIntentHandler">Execute intent handler.</param>
        /// <param name="disconnectIntentHandler">Disconnect intent handler.</param>
        public GoogleHomeController(
            SyncIntentHandler syncIntentHandler,
            QueryIntentHandler queryIntentHandler,
            ExecuteIntentHandler executeIntentHandler,
            DisconnectIntentHandler disconnectIntentHandler)
        {
            _disconnectIntentHandler = disconnectIntentHandler ?? throw new ArgumentNullException(nameof(disconnectIntentHandler));
            _syncIntentHandler = syncIntentHandler ?? throw new ArgumentNullException(nameof(syncIntentHandler));
            _queryIntentHandler = queryIntentHandler ?? throw new ArgumentNullException(nameof(queryIntentHandler));
            _executeIntentHandler = executeIntentHandler ?? throw new ArgumentNullException(nameof(executeIntentHandler));
        }

        /// <summary>
        /// Post handler.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Post([FromBody] Request request)
        {
            // Smart Home Intents use a single object in inputs, containing the intent value, and a payload object with automation-specific objects.
            if (request.Inputs.Count != 1)
            {
                return BadRequest(new Models.Response.Response
                {
                    RequestId = request.RequestId,
                    Payload = new Models.Response.ErrorResponsePayload { ErrorCode = "protocolError" }
                });
            }

            var input = request.Inputs.First();
            switch (input)
            {
                case SyncIntent syncIntent:
                    return Ok(new Models.Response.Response
                    {
                        RequestId = request.RequestId,
                        Payload = _syncIntentHandler.Handle(syncIntent)
                    });
                case QueryIntent queryIntent:
                    return Ok(new Models.Response.Response
                    {
                        RequestId = request.RequestId,
                        Payload = _queryIntentHandler.Handle(queryIntent)
                    });
                case ExecuteIntent executeIntent:
                    return Ok(new Models.Response.Response
                    {
                        RequestId = request.RequestId,
                        Payload = _executeIntentHandler.Handle(executeIntent)
                    });
                case DisconnectIntent disconnectIntent:
                    _disconnectIntentHandler.Handle(disconnectIntent);
                    return Ok();
            }

            // No valid intents found, return error
            return BadRequest(new Models.Response.Response
            {
                RequestId = request.RequestId,
                Payload = new Models.Response.ErrorResponsePayload { ErrorCode = "protocolError" }
            });
        }
    }
}
