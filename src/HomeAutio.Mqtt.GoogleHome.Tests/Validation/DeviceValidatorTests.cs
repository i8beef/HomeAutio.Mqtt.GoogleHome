using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.App_Start;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Validation;
using Newtonsoft.Json;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Validation
{
    public class DeviceValidatorTests
    {
        private readonly string _testFilePath;

        public DeviceValidatorTests()
        {
            // Global JSON options
            JsonSerializerConfig.Configure();

            // Repository
            _testFilePath = "TestData/googleDevices.json";
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void CanValidate()
        {
            // Arrange
            if (File.Exists(_testFilePath))
            {
                var deviceConfigurationString = File.ReadAllText(_testFilePath);
                var devices = new ConcurrentDictionary<string, Device>(JsonConvert.DeserializeObject<Dictionary<string, Device>>(deviceConfigurationString));

                // Act
                var errors = devices.Select(x => DeviceValidator.Validate(x.Value));

                // Assert
                Assert.All(errors, result => Assert.Empty(result));
            }
        }
    }
}
