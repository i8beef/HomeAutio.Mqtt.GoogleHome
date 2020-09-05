using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Identity in memory resources.
    /// </summary>
    internal class Resources
    {
        /// <summary>
        /// Gets static list of identity resources based on configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of <see cref="IdentityResource"/>.</returns>
        public static IEnumerable<IdentityResource> GetIdentityResources(IConfiguration configuration)
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        /// <summary>
        /// Gets static list of api resources based on configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of <see cref="ApiResource"/>.</returns>
        public static IEnumerable<ApiResource> GetApiResources(IConfiguration configuration)
        {
            var clientsSection = configuration.GetSection("oauth:resources");

            return clientsSection.GetChildren()
                .Select(x => new ApiResource
                {
                    Name = x.GetValue<string>("resourceName"),
                    DisplayName = x.GetValue<string>("resourceName"),
                    Description = x.GetValue<string>("resourceName"),
                    UserClaims = new List<string>(),
                    ApiSecrets = new List<Secret>(),
                    Scopes = new List<string> { "api" }
                });
        }
    }
}
