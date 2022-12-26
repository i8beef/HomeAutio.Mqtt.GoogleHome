using HomeAutio.Mqtt.GoogleHome.Models.State;
using System;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State
{
    public class DeviceStateTests
    {
        [Theory]
        [InlineData(GoogleType.Numeric, "1", 1, typeof(int))]
        [InlineData(GoogleType.Numeric, "2.5", 2.5, typeof(decimal))]
        [InlineData(GoogleType.Bool, "true", true, typeof(bool))]
        [InlineData(GoogleType.String, "true", "true", typeof(string))]
        [InlineData(GoogleType.Numeric, "SaladFork", default(int), typeof(int))]
        [InlineData(GoogleType.Bool, "SaladFork", default(bool), typeof(bool))]
        public void MapValueToGoogleConvertsTypes(GoogleType googleType, string value, object expected, Type expectedType)
        {
            // Arrange
            var deviceState = new DeviceState { Topic = "test" };

            // Hack for decimal constant values for InlineData
            if (expected is double)
            {
                expected = Convert.ToDecimal(expected);
            }

            // Act
            var result = deviceState.MapValueToGoogle(value, googleType)!;

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expectedType, result.GetType());
        }
    }
}
