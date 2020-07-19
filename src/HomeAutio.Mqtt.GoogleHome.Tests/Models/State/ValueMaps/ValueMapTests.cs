using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State.ValueMaps
{
    public class ValueMapTests
    {
        [Theory]
        [InlineData("mqtt", "google", "google", "mqtt")]
        [InlineData("2", "1", 1, "2")]
        public void CanMapToMqtt(string mqttValue, string googleValue, object value, string expectedResult)
        {
            // Arrange
            var mapper = new ValueMap
            {
                Google = googleValue,
                Mqtt = mqttValue
            };

            // Act
            var matches = mapper.MatchesGoogle(value);
            var result = mapper.ConvertToMqtt(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("mqtt", "google", "mqtt", "google")]
        public void CanMapToGoogle(string mqttValue, string googleValue, string value, string expectedResult)
        {
            // Arrange
            var mapper = new ValueMap
            {
                Google = googleValue,
                Mqtt = mqttValue
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
