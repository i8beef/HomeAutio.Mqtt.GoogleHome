using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.App_Start;
using HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests
{
    public class GoogleHomeGraphClientTests
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<GoogleHomeGraphClient>> _loggerMock;

        private readonly string _agentUserId;
        private readonly ServiceAccount _serviceAccount;

        public GoogleHomeGraphClientTests()
        {
            // Mocks
            _loggerMock = new Mock<ILogger<GoogleHomeGraphClient>>();
            _httpClient = new HttpClient();

            // Global JSON options
            JsonSerializerConfig.Configure();

            // Service account setup
            _agentUserId = TestHelper.Configuration["agentUserId"];
            _serviceAccount = JsonConvert.DeserializeObject<ServiceAccount>(File.ReadAllText(@"TestData/serviceAccount.json"));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CanCallRequestSync()
        {
            // Only run if integration test requirements are present
            if (_agentUserId != "AGENT USER ID" && _serviceAccount.ProjectId != "PROJECT ID")
            {
                // Arrange
                var client = new GoogleHomeGraphClient(_loggerMock.Object, _httpClient, _serviceAccount, _agentUserId);

                // Act
                await client.RequestSyncAsync();

                // Assert
                _loggerMock.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Never);
            }
        }
    }
}
