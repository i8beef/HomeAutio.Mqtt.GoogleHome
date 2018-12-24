using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        private readonly SyncIntentHandler _syncIntentHandler;
        private readonly QueryIntentHandler _queryIntentHandler;
        private readonly ExecuteIntentHandler _executeIntentHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeController"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="syncIntentHandler">Sync intent handler.</param>
        /// <param name="queryIntentHandler">Query ntent handler.</param>
        /// <param name="executeIntentHandler">Execute intent handler.</param>
        public GoogleHomeController(
            ILogger<GoogleHomeController> logger,
            SyncIntentHandler syncIntentHandler,
            QueryIntentHandler queryIntentHandler,
            ExecuteIntentHandler executeIntentHandler)
        {
            _log = logger;

            _syncIntentHandler = syncIntentHandler;
            _queryIntentHandler = queryIntentHandler;
            _executeIntentHandler = executeIntentHandler;
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
                    response.Payload = _syncIntentHandler.Handle(syncIntent);
                    return Ok(response);
                case Models.Request.QueryIntent queryIntent:
                    response.Payload = _queryIntentHandler.Handle(queryIntent);
                    return Ok(response);
                case Models.Request.ExecuteIntent executeIntent:
                    response.Payload = _executeIntentHandler.Handle(executeIntent);
                    return Ok(response);
                case Models.Request.DisconnectIntent disconnectIntent:
                    return Ok();
            }

            // No valid intents found, return error
            response.Payload = new Models.Response.ErrorResponsePayload { ErrorCode = "protocolError" };
            return BadRequest(response);
        }
    }
}
