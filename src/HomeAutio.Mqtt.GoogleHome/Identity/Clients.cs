using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Identity in memory clients.
    /// </summary>
    internal class Clients
    {
        /// <summary>
        /// Gets static in memory clients based on configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of <see cref="Client"/>.</returns>
        public static IEnumerable<Client> Get(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = configuration.GetValue<string>("oauth:clientId"),
                    ClientName = "Google Actions Client",
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    ClientSecrets = new List<Secret> { new Secret(configuration.GetValue<string>("oauth:clientSecret").Sha256()) },
                    AllowedScopes = new List<string> { "api" },
                    RedirectUris = new List<string> { configuration.GetValue<string>("oauth:redirectUri") }
                }
            };
        }
    }
}
