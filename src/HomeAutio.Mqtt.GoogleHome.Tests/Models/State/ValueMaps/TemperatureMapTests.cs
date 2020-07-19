using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State.ValueMaps
{
    public class TemperatureMapTests
    {
        [Theory]
        [InlineData(0, "32")]
        public void CanMapToMqtt(object value, string expectedResult)
        {
            // Arrange
            var mapper = new TemperatureMap();

            // Act
            var matches = mapper.MatchesGoogle(value);
            var result = mapper.ConvertToMqtt(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("32", "0")]
        public void CanMapToGoogle(string value, string expectedResult)
        {
            // Arrange
            var mapper = new TemperatureMap();

            // Act
            var matches = mapper.MatchesMqtt(value);
            var result = mapper.ConvertToGoogle(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }
    }
}
