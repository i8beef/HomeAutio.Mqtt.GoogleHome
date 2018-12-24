using HomeAutio.Mqtt.GoogleHome.Models;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models
{
    public class CommandToStateKeyMapperTests
    {
        [Theory]
        [InlineData("updateModeSettings.temperature", "currentModeSettings.temperature")]
        [InlineData("updateToggleSettings.sterilization", "currentToggleSettings.sterilization")]
        [InlineData("fanSpeed", "currentFanSpeedSetting")]
        [InlineData("color.temperature", "color.temperatureK")]
        public void CanMap(string input, string expectedOutput)
        {
            // Act
            var result = CommandToStateKeyMapper.Map(input);

            // Assert
            Assert.Equal(expectedOutput, result);
        }
    }
}
