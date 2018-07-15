using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    internal class Users
    {
        public static List<TestUser> Get(IConfiguration configuration)
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = configuration.GetValue<string>("oauth:subjectId"),
                    Username = configuration.GetValue<string>("oauth:username"),
                    Password = configuration.GetValue<string>("oauth:password"),
                    Claims = new List<Claim>()
                }
            };
        }
    }
}
