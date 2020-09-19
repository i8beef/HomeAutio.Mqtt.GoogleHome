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
        /// Gets static list of api scopes.
        /// </summary>
        /// <returns>A list of <see cref="ApiScope"/>.</returns>
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "api", displayName: "API")
            };
        }
    }
}
