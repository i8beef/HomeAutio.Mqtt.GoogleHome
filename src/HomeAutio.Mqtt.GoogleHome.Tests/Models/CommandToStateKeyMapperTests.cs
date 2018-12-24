using HomeAutio.Mqtt.GoogleHome.Models;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models
{
    public class CommandToStateKeyMapperTests
    {
        [Theory]
        [InlineData("updateModeSettings.temperature", "currentModeSettings.temperature")]
        [InlineData("updateToggleSettings.sterilization", "currentToggleSettings.sterilization")]
        [InlineData("color.temperature", "color.temperatureK")]
        [InlineData("color.spectrumRGB", "color.spectrumRgb")]
        [InlineData("color.spectrumHSV.hue", "color.spectrumHsv.hue")]
        [InlineData("color.spectrumHSV.saturation", "color.spectrumHsv.saturation")]
        [InlineData("color.spectrumHSV.value", "color.spectrumHsv.value")]
        [InlineData("fanSpeed", "currentFanSpeedSetting")]
        [InlineData("start", "isRunning")]
        [InlineData("pause", "isPaused")]
        public void CanMap(string input, string expectedOutput)
        {
            // Act
            var result = CommandToStateKeyMapper.Map(input);

            // Assert
            Assert.Equal(expectedOutput, result);
        }
    }
}
