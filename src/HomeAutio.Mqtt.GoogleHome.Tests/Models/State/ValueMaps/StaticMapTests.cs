using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State.ValueMaps
{
    public class StaticMapTests
    {
        [Theory]
        [InlineData("test", null, null)]
        public void CanMapToMqtt(string googleValue, object value, string expectedResult)
        {
            // Arrange
            var mapper = new StaticMap
            {
                Google = googleValue
            };

            // Act
            var matches = mapper.MatchesGoogle(value);
            var result = mapper.ConvertToMqtt(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test", null, "test")]
        public void CanMapToGoogle(string googleValue, string value, string expectedResult)
        {
            // Arrange
            var mapper = new StaticMap
            {
                Google = googleValue
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
