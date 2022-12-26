using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Identity in memory clients.
    /// </summary>
    public class Clients
    {
        /// <summary>
        /// Gets static in memory clients based on configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of <see cref="Client"/>.</returns>
        public static IEnumerable<Client> Get(IConfiguration configuration)
        {
            var clientsSection = configuration.GetSection("oauth:clients");
            var clients = clientsSection.GetChildren()
                .Select(x => new Client
                {
                    ClientId = x.GetValue<string>("clientId"),
                    ClientName = x.GetValue<string>("clientName"),
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = false,
                    ClientSecrets = new List<Secret> { new Secret(x.GetValue<string>("clientSecret").Sha256()) },
                    AllowedScopes = new List<string> { "api" },
                    RedirectUris = x.GetSection("allowedRedirectUris").GetChildren().Select(uri => uri.Value).ToList(),
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = x.GetValue("refreshTokenReuse", false)
                        ? TokenUsage.ReUse
                        : TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = x.GetValue("refreshTokenLifetime", 30) * 86400,
                    AbsoluteRefreshTokenLifetime = 0
                });

            return clients;
        }
    }
}
