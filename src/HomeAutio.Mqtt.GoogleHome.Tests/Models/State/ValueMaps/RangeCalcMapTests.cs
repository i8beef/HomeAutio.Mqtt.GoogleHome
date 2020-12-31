using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State.ValueMaps
{
    public class RangeCalcMapTests
    {
        [Theory]
        [InlineData(0, "0", 0d, 100d, 0d, 254d)]
        [InlineData(50, "127", 0d, 100d, 0d, 254d)]
        [InlineData(100, "254", 0d, 100d, 0d, 254d)]
        public void CanMapToMqtt(object value, string expectedResult, decimal googleMin, decimal googleMax, decimal mqttMin, decimal mqttMax)
        {
            // Arrange
            var mapper = new LinearRangeMap
            {
                GoogleMin = googleMin,
                GoogleMax = googleMax,
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
        [InlineData("0", "0", 0d, 100d, 0d, 254d)]
        [InlineData("127", "50", 0d, 100d, 0d, 254d)]
        [InlineData("254", "100", 0d, 100d, 0d, 254d)]
        public void CanMapToGoogle(string value, string expectedResult, decimal googleMin, decimal googleMax, decimal mqttMin, decimal mqttMax)
        {
            // Arrange
            var mapper = new LinearRangeMap
            {
                GoogleMin = googleMin,
                GoogleMax = googleMax,
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
