using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Google Home Graph API client.
    /// </summary>
    public class GoogleHomeGraphClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _agentUserId;
        private readonly ServiceAccount _serviceAccount;

        private AccessTokenResponse _accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleHomeGraphClient"/> class.
        /// </summary>
        /// <param name="httpClientFactory">HttpClient factory.</param>
        /// <param name="serviceAccount">Service account information.</param>
        /// <param name="agentUserId">Agent user id.</param>
        public GoogleHomeGraphClient(IHttpClientFactory httpClientFactory, ServiceAccount serviceAccount, string agentUserId)
        {
            _httpClientFactory = httpClientFactory;
            _agentUserId = agentUserId;
            _serviceAccount = serviceAccount;
        }

        /// <summary>
        /// Send updates to the Google Home Graph.
        /// </summary>
        /// <param name="devices">Devices updated.</param>
        /// <param name="stateCache">Current state cache.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task SendUpdatesAsync(IList<Models.State.Device> devices, IDictionary<string, string> stateCache)
        {
            // Ensure access token is available
            if (_accessToken == null || _accessToken.ExpiresAt <= DateTime.Now)
            {
                _accessToken = await GetAccessToken(ConstructJwt());
            }

            var requestMessage = new Request
            {
                RequestId = Guid.NewGuid().ToString(),
                AgentUserId = _agentUserId,
                Payload = new QueryResponsePayload
                {
                    Devices = new Devices
                    {
                        States = devices.ToDictionary(
                            device => device.Id,
                            device => device.GetGoogleState(stateCache))
                    }
                }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://homegraph.googleapis.com/v1/devices:reportStateAndNotification"),
                Content = new StringContent(JsonConvert.SerializeObject(requestMessage))
            };

            // Add access token
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken.AccessToken);

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
        }

        /// <summary>
        /// Gets an access token using the passed JWT request.
        /// </summary>
        /// <param name="jwt">JWT request.</param>
        /// <returns>An <see cref="AccessTokenResponse"/>.</returns>
        private async Task<AccessTokenResponse> GetAccessToken(string jwt)
        {
            var paramaters = new Dictionary<string, string>();
            paramaters.Add("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");
            paramaters.Add("assertion", ConstructJwt());

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_serviceAccount.TokenUri),
                Content = new FormUrlEncodedContent(paramaters)
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            return await response.Content.ReadAsAsync<AccessTokenResponse>();
        }

        /// <summary>
        /// Gets a JWT token.
        /// </summary>
        /// <returns>A JWT token.</returns>
        private string ConstructJwt()
        {
            // Get signing key
            byte[] certBuffer = GetBytesFromPEM(_serviceAccount.PrivateKey, "PRIVATE KEY");
            var securityKey = new SymmetricSecurityKey(certBuffer);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);

            // Create auth token
            var claims = new List<Claim> { new Claim("scope", "https://www.googleapis.com/auth/homegraph") };
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

            return token;
        }

        /// <summary>
        /// Extracts key parts from a PEM string.
        /// </summary>
        /// <param name="pemString">String to extract.</param>
        /// <param name="section">Section of key to extract.</param>
        /// <returns>The extracted portion as a byte array.</returns>
        private byte[] GetBytesFromPEM(string pemString, string section)
        {
            var header = string.Format("-----BEGIN {0}-----", section);
            var footer = string.Format("-----END {0}-----", section);

            var start = pemString.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0)
                return null;

            return Convert.FromBase64String(pemString.Substring(start, end));
        }
    }
}
