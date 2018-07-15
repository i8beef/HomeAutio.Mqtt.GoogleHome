using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources(IConfiguration configuration)
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources(IConfiguration configuration)
        {
            return new List<ApiResource> {
                new ApiResource {
                    Name = configuration.GetValue<string>("oauth:resourceName"),
                    DisplayName = configuration.GetValue<string>("oauth:resourceName"),
                    Description = configuration.GetValue<string>("oauth:resourceName"),
                    UserClaims = new List<string>(),
                    ApiSecrets = new List<Secret> { new Secret(configuration.GetValue<string>("oauth:clientSecret").Sha256()) },
                    Scopes = new List<Scope> { new Scope("api") }
                }
            };
        }
    }
}
