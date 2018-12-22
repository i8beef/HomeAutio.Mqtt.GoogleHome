using System.Net.Http;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests
{
    public class GoogleHomeGraphClientTests
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<GoogleHomeGraphClient>> _loggerMock;

        public GoogleHomeGraphClientTests()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                settings.Converters.Add(new StringEnumConverter());

                return settings;
            };

            _loggerMock = new Mock<ILogger<GoogleHomeGraphClient>>();
            _httpClient = new HttpClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CanCallRequestSync()
        {
            // Arrange
            var serviceAccountString = @"{
  ""type"": ""service_account"",
  ""project_id"": ""PROJECT ID"",
  ""private_key_id"": ""PRIVATE KEY ID"",
  ""private_key"": ""PRIVATE KEY"",
  ""client_email"": ""SERVICE ACCOUNT EMAIL"",
  ""client_id"": ""CLIENT ID"",
  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
  ""token_uri"": ""https://accounts.google.com/o/oauth2/token"",
  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
  ""client_x509_cert_url"": ""CLIENT CERT URL""
}";

            var serviceAccount = JsonConvert.DeserializeObject<ServiceAccount>(serviceAccountString);
            var agentUserId = "AGENT USER ID";

            var client = new GoogleHomeGraphClient(_loggerMock.Object, _httpClient, serviceAccount, agentUserId);

            // Act
            await client.RequestSyncAsync();

            // Assert
            _loggerMock.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Never);
        }
    }
}
