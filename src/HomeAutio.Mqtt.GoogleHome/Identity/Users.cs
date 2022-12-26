using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Identity in memory users.
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Gets static list of in memory users based on configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of <see cref="TestUser"/>.</returns>
        public static List<TestUser> Get(IConfiguration configuration)
        {
            var usersSection = configuration.GetSection("oauth:users");

            return usersSection.GetChildren()
                .Select(x => new TestUser
                {
                    SubjectId = x.GetValue<string>("subjectId"),
                    Username = x.GetValue<string>("username"),
                    Password = x.GetValue<string>("password"),
                    Claims = new List<Claim>()
                })
                .ToList();
        }
    }
}
