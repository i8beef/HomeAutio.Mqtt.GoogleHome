using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State.ValueMaps
{
    public class RegexMapTests
    {
        [Theory]
        [InlineData("^mqtt", "mqttResult", "^google$", "googleResult", "google", "mqttResult")]
        [InlineData(null, "mqttResult", "^google$", null, "google", "mqttResult")]
        [InlineData(null, null, "^google$", null, "google", "google")]
        [InlineData("^255$", "255", "^100", "100", 100, "255")]
        public void CanMapToMqtt(string mqttSearch, string mqttReplace, string googleSearch, string googleReplace, object value, string expectedResult)
        {
            // Arrange
            var mapper = new RegexMap
            {
                MqttSearch = mqttSearch,
                MqttReplace = mqttReplace,
                GoogleSearch = googleSearch,
                GoogleReplace = googleReplace
            };

            // Act
            var matches = mapper.MatchesGoogle(value);
            var result = mapper.ConvertToMqtt(value);

            // Assert
            Assert.True(matches);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("^mqtt$", "mqttResult", "^google$", "googleResult", "mqtt", "googleResult")]
        [InlineData("^mqtt", null, null, "googleResult", "mqtt", "googleResult")]
        [InlineData("^mqtt", null, null, null, "mqtt", "mqtt")]
        [InlineData("^255", "255", "^100", "100", "255", "100")]
        public void CanMapToGoogle(string mqttSearch, string mqttReplace, string googleSearch, string googleReplace, string value, string expectedResult)
        {
            // Arrange
            var mapper = new RegexMap
            {
                MqttSearch = mqttSearch,
                MqttReplace = mqttReplace,
                GoogleSearch = googleSearch,
                GoogleReplace = googleReplace
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
