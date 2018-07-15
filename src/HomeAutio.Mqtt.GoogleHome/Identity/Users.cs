using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Identity in memory users.
    /// </summary>
    internal class Users
    {
        /// <summary>
        /// Gets static list of in memory users based on configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of <see cref="TestUser"/>.</returns>
        public static List<TestUser> Get(IConfiguration configuration)
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = configuration.GetValue<string>("oauth:subjectId"),
                    Username = configuration.GetValue<string>("oauth:username"),
                    Password = configuration.GetValue<string>("oauth:password"),
                    Claims = new List<Claim>()
                }
            };
        }
    }
}
