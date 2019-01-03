using System.Collections.Generic;
using System.IO;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.App_Start;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Tests.TestData;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests
{
    [TestCaseOrderer("HomeAutio.Mqtt.GoogleHome.Tests.PriorityOrderer", "HomeAutio.Mqtt.GoogleHome.Tests")]
    public class GoogleDeviceRepositoryTests
    {
        private readonly string _testFilePath;
        private readonly Mock<ILogger<GoogleDeviceRepository>> _logMock;
        private readonly Mock<IMessageHub> _messageHubMock;

        public GoogleDeviceRepositoryTests()
        {
            // mocks
            _logMock = new Mock<ILogger<GoogleDeviceRepository>>();
            _messageHubMock = new Mock<IMessageHub>();

            // Global JSON options
            JsonSerializerConfig.Configure();

            // Repository
            _testFilePath = "TestData/googleDevices.json";
        }

        [Fact]
        [Trait("Category", "Integration")]
        [TestPriority(0)]
        public void CanAddItem()
        {
            // Arrange
            var device = DeviceTestData.FullDevice();
            device.Id = device.Id + "1";

            var expectedAddedTopics = device.Traits
                .SelectMany(trait => trait.State)
                .Where(x => x.Value.Topic != null)
                .Select(x => x.Value.Topic);

            var repository = new GoogleDeviceRepository(_logMock.Object, _messageHubMock.Object, _testFilePath);

            // Act
            repository.Add(device);
            var deviceConfigurationString = File.ReadAllText(_testFilePath);
            var result = new Dictionary<string, Device>(JsonConvert.DeserializeObject<Dictionary<string, Device>>(deviceConfigurationString));

            // Assert
            Assert.True(repository.Contains(device.Id));            
            Assert.True(result.ContainsKey(device.Id));

            foreach (var expectedTopic in expectedAddedTopics)
            {
                _messageHubMock.Verify(x => x.Publish(It.Is<ConfigSubscriptionChangeEvent>(changeEvent => changeEvent.AddedSubscriptions.Contains(expectedTopic))), Times.Exactly(1));
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        [TestPriority(1)]
        public void CanUpdateItem()
        {
            // Arrange
            var device = DeviceTestData.FullDevice();
            var oldDeviceId = device.Id + "1";

            var expectedDeletedTopics = device.Traits
                .SelectMany(trait => trait.State)
                .Where(x => x.Value.Topic != null)
                .Select(x => x.Value.Topic)
                .ToList();

            device.Traits.Clear();
            device.Traits.Add(new DeviceTrait {
                Trait = GoogleHome.Models.TraitType.Brightness,
                Commands = new Dictionary<string, IDictionary<string, string>>
                {
                    {
                        "action.devices.commands.BrightnessAbsolute",
                        new Dictionary<string, string>
                        {
                            {
                                "brightness",
                                "test/device/brightness/set"
                            }
                        }
                    }
                },
                State = new Dictionary<string, DeviceState>
                {
                    {
                        "brightness",
                        new DeviceState
                        {
                            Topic = "/test/device/brightness",
                            GoogleType = GoogleType.Numeric,
                            ValueMap = null
                        }
                    }
                }
            });

            var expectedAddedTopics = device.Traits
                .SelectMany(trait => trait.State)
                .Where(x => x.Value.Topic != null)
                .Select(x => x.Value.Topic);

            var repository = new GoogleDeviceRepository(_logMock.Object, _messageHubMock.Object, _testFilePath);

            // Act
            repository.Update(oldDeviceId, device);
            var deviceConfigurationString = File.ReadAllText(_testFilePath);
            var result = new Dictionary<string, Device>(JsonConvert.DeserializeObject<Dictionary<string, Device>>(deviceConfigurationString));

            // Assert
            Assert.True(!repository.Contains(oldDeviceId));
            Assert.True(repository.Contains(device.Id));

            Assert.True(!result.ContainsKey(oldDeviceId));
            Assert.True(result.ContainsKey(device.Id));

            foreach (var expectedTopic in expectedAddedTopics)
            {
                _messageHubMock.Verify(x => x.Publish(It.Is<ConfigSubscriptionChangeEvent>(changeEvent => changeEvent.AddedSubscriptions.Contains(expectedTopic))), Times.Exactly(1));
            }

            foreach (var expectedTopic in expectedDeletedTopics)
            {
                _messageHubMock.Verify(x => x.Publish(It.Is<ConfigSubscriptionChangeEvent>(changeEvent => changeEvent.DeletedSubscriptions.Contains(expectedTopic))), Times.Exactly(1));
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        [TestPriority(2)]
        public void CanDeleteItem()
        {
            // Arrange
            var repository = new GoogleDeviceRepository(_logMock.Object, _messageHubMock.Object, _testFilePath);
            var device = repository.Get(DeviceTestData.FullDevice().Id);
            var expectedDeletedTopics = device.Traits
                .SelectMany(trait => trait.State)
                .Where(x => x.Value.Topic != null)
                .Select(x => x.Value.Topic);

            // Act
            repository.Delete(device.Id);
            var deviceConfigurationString = File.ReadAllText(_testFilePath);
            var result = new Dictionary<string, Device>(JsonConvert.DeserializeObject<Dictionary<string, Device>>(deviceConfigurationString));

            // Assert
            Assert.True(!repository.Contains(device.Id));
            Assert.True(!result.ContainsKey(device.Id));

            foreach (var expectedTopic in expectedDeletedTopics)
            {
                _messageHubMock.Verify(x => x.Publish(It.Is<ConfigSubscriptionChangeEvent>(changeEvent => changeEvent.DeletedSubscriptions.Contains(expectedTopic))), Times.Exactly(1));
            }
        }
    }
}
