using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Google Home Graph API client.
    /// </summary>
    public class GoogleHomeGraphClient
    {
        private const string _googleHomeGraphApiUri = "https://homegraph.googleapis.com/v1";
        private const string _homeGraphScope = "https://www.googleapis.com/auth/homegraph";

        private readonly ILogger<GoogleHomeGraphClient> _log;
        private readonly HttpClient _httpClient;
        private readonly string _agentUserId;
        private readonly ServiceAccount _serviceAccount;
        private readonly object _tokenRefreshLock = new object();

        private AccessTokenResponse _accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeGraphClient"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="httpClient">HttpClient factory.</param>
        /// <param name="serviceAccount">Service account information.</param>
        /// <param name="agentUserId">Agent user id.</param>
        public GoogleHomeGraphClient(
            ILogger<GoogleHomeGraphClient> logger,
            HttpClient httpClient,
            ServiceAccount serviceAccount,
            string agentUserId)
        {
            _log = logger ?? throw new ArgumentException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentException(nameof(httpClient));

            _agentUserId = agentUserId;
            _serviceAccount = serviceAccount;
        }

        /// <summary>
        /// Send Google Home Graph request sync.
        /// </summary>
        /// <param name="isAsync">Indicates if the sync request should return immediately or wait for a response.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task RequestSyncAsync(bool isAsync = true)
        {
            // If no service account has been provided, don't attempt to call
            if (_serviceAccount == null)
            {
                _log.LogWarning("REQUEST_SYNC triggered but Google Home Graph serviceAccountFile setting was blank, or the file didn't exist");
                return;
            }

            var request = new RequestSyncRequest
            {
                AgentUserId = _agentUserId,
                Async = isAsync
            };

            var serializedContent = JsonConvert.SerializeObject(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_googleHomeGraphApiUri + "/devices:requestSync"),
                Content = new StringContent(serializedContent, Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(requestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _log.LogInformation("Google Home Graph REQUEST_SYNC sent");
            }
            else
            {
                _log.LogWarning("Google Home Graph REQUEST_SYNC failed");
            }
        }

        /// <summary>
        /// Send updates to the Google Home Graph.
        /// </summary>
        /// <param name="devices">Devices updated.</param>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task SendUpdatesAsync(IList<Models.State.Device> devices, IDictionary<string, string> stateCache)
        {
            // If no service account has been provided, don't attempt to call
            if (_serviceAccount == null)
            {
                _log.LogWarning("WillReportState triggered but Google Home Graph serviceAccountFile setting was blank, or the file didn't exist");
                return;
            }

            // Ensure Google is given a complete state representation
            var unintializedDevices = devices.Where(x => x.IsStateFullyInitialized(stateCache) == false);
            if (unintializedDevices.Any())
            {
                _log.LogWarning("WillReportState triggered but state cache not fully initialized: {Devices}", unintializedDevices.Select(x => x.Id));
                return;
            }

            var request = new ReportStateAndNotificationRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                EventId = Guid.NewGuid().ToString(),
                AgentUserId = _agentUserId,
                Payload = new StateAndNotificationPayload
                {
                    Devices = new ReportStateAndNotificationDevice
                    {
                        States = devices.ToDictionary(
                            device => device.Id,
                            device => device.GetGoogleState(stateCache)),
                        Notifications = null
                    }
                }
            };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_googleHomeGraphApiUri + "/devices:reportStateAndNotification"),
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(requestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _log.LogInformation("Google Home Graph updated for devices: " + string.Join(", ", devices.Select(x => x.Id)));
            }
            else
            {
                _log.LogWarning("Google Home Graph update failed for devices: " + string.Join(", ", devices.Select(x => x.Id)));
            }
        }

        /// <summary>
        /// Gets a JWT token.
        /// </summary>
        /// <returns>A JWT token.</returns>
        private string CreateJwt()
        {
            // Get signing key
            var rsaSecurityKey = GetGoogleHomeGraphApiSigningKey();
            var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            // Create auth token
            var claims = new List<Claim> { new Claim("scope", _homeGraphScope) };
            var header = new JwtHeader(signingCredentials);
            var payload = new JwtPayload(
                _serviceAccount.ClientEmail,
                _serviceAccount.TokenUri,
                claims,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                DateTime.Now);
            var jwtToken = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtToken);

            _log.LogDebug("Built JWT request: " + token);

            return token;
        }

        /// <summary>
        /// Gets an access token using the passed JWT request.
        /// </summary>
        /// <returns>An <see cref="AccessTokenResponse"/>.</returns>
        private async Task<AccessTokenResponse> GetAccessToken()
        {
            _log.LogDebug("Get/Refresh access token");

            var paramaters = new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "assertion", CreateJwt() }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_serviceAccount.TokenUri),
                Content = new FormUrlEncodedContent(paramaters)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<AccessTokenResponse>(content);

            _log.LogDebug("Received access token: " + accessToken);

            return accessToken;
        }

        /// <summary>
        /// Gets the Google Home Graph API signing key.
        /// </summary>
        /// <returns>A <see cref="RsaSecurityKey"/> for building an RSA key.</returns>
        private RsaSecurityKey GetGoogleHomeGraphApiSigningKey()
        {
            if (_serviceAccount == null)
                throw new ArgumentException("Google Home Graph serviceAccountFile blank or missing");

            using (var stringReader = new StringReader(_serviceAccount.PrivateKey))
            {
                var reader = new PemReader(stringReader);
                var key = (RsaPrivateCrtKeyParameters)reader.ReadObject();
                var parameters = DotNetUtilities.ToRSAParameters(key);

                return new RsaSecurityKey(parameters);
            }
        }

        /// <summary>
        /// Sends a HomeGraph API request.
        /// </summary>
        /// <param name="requestMessage">Request message.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage)
        {
            // Ensure access token is available
            if (_accessToken == null || _accessToken.ExpiresAt <= DateTime.Now.AddMinutes(-1))
            {
                lock (_tokenRefreshLock)
                {
                    // Recheck in case another instance already updated
                    if (_accessToken == null || _accessToken.ExpiresAt <= DateTime.Now.AddMinutes(-1))
                    {
                        _accessToken = GetAccessToken()
                            .GetAwaiter().GetResult();
                    }
                }
            }

            // Add access token
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken.AccessToken);

            return await _httpClient.SendAsync(requestMessage);
        }
    }
}
