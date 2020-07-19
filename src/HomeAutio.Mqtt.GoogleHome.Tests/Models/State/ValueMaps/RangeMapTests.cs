using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State.ValueMaps
{
    public class RangeMapTests
    {
        [Theory]
        [InlineData(1, 100, "1", 1, "1")]
        public void CanMapToMqtt(decimal mqttMin, decimal mqttMax, string googleValue, object value, string expectedResult)
        {
            // Arrange
            var mapper = new RangeMap
            {
                Google = googleValue,
                MqttMin = mqttMin,
                MqttMax = mqttMax
            };

            // Act
            var matches = mapper.MatchesGoogle(value);
            var result = mapper.ConvertToMqtt(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(1, 100, "on", "1", "on")]
        public void CanMapToGoogle(decimal mqttMin, decimal mqttMax, string googleValue, string value, string expectedResult)
        {
            // Arrange
            var mapper = new RangeMap
            {
                Google = googleValue,
                MqttMin = mqttMin,
                MqttMax = mqttMax
            };

            // Act
            var matches = mapper.MatchesMqtt(value);
            var result = mapper.ConvertToGoogle(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }
    }
}
