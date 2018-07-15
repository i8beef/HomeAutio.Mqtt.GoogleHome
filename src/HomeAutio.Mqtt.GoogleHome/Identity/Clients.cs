using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    internal class Clients
    {
        public static IEnumerable<Client> Get(IConfiguration configuration)
        {
            return new List<Client> {
                new Client {
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
