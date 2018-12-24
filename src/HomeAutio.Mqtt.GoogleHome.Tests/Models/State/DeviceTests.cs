using HomeAutio.Mqtt.GoogleHome.Models.State;
using System.Collections.Generic;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Models.State
{
    public class DeviceTests
    {
        [Fact]
        public void GetsGoogleStateForNestedParameters()
        {
            // Arrange
            var device = new Device
            {
                Traits = new List<DeviceTrait>
                {
                    new DeviceTrait
                    {
                        Trait = GoogleHome.Models.TraitType.ColorSetting,
                        Commands = new Dictionary<string, IDictionary<string, string>>
                        {
                            {
                                "action.devices.commands.ColorAbsolute",
                                new Dictionary<string, string>
                                {
                                    { "color.name", "device/color/name/set" },
                                    { "color.temperature", "device/color/temperature/set" },
                                    { "color.spectrumRGB", "device/color/spectrumRGB/set" },
                                    { "color.spectrumHSV.hue", "device/color/spectrumHSV/hue/set" },
                                    { "color.spectrumHSV.saturation", "device/color/spectrumHSV/saturation/set" },
                                    { "color.spectrumHSV.value", "device/color/spectrumHSV/value/set" }
                                }
                            }
                        },
                        State = new Dictionary<string, DeviceState>
                        {
                            {
                                "color.name",
                                new DeviceState
                                {
                                    Topic = "device/color/name",
                                    GoogleType = GoogleType.String,
                                    ValueMap = null
                                }
                            },
                            {
                                "color.temperature",
                                new DeviceState
                                {
                                    Topic = "device/color/temperature",
                                    GoogleType = GoogleType.Numeric,
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumRGB",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumRGB",
                                    GoogleType = GoogleType.Numeric,
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumHSV.hue",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumHSV/hue",
                                    GoogleType = GoogleType.Numeric,
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumHSV.saturation",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumHSV/saturation",
                                    GoogleType = GoogleType.Numeric,
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumHSV.value",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumHSV/value",
                                    GoogleType = GoogleType.Numeric,
                                    ValueMap = null
                                }
                            }
                        }
                    }
                }
            };

            var stateCache = new Dictionary<string, string>
            {
                { "device/color/name", "name" },
                { "device/color/temperature", "85" },
                { "device/color/spectrumRGB", "12345" },
                { "device/color/spectrumHSV/hue", "123" },
                { "device/color/spectrumHSV/saturation", "456" },
                { "device/color/spectrumHSV/value", "789" },
            };

            // Act
            var result = device.GetGoogleState(stateCache);

            // Assert
            Assert.True(result.ContainsKey("color"));

            var colorResult = (IDictionary<string, object>)result["color"];
            Assert.True(colorResult.ContainsKey("spectrumHSV"));

            var spectrumHSVResult = (IDictionary<string, object>)colorResult["spectrumHSV"];
            Assert.True(spectrumHSVResult.ContainsKey("hue"));
            Assert.Equal(123, spectrumHSVResult["hue"]);
            Assert.True(spectrumHSVResult.ContainsKey("saturation"));
            Assert.Equal(456, spectrumHSVResult["saturation"]);
            Assert.True(spectrumHSVResult.ContainsKey("value"));
            Assert.Equal(789, spectrumHSVResult["value"]);
        }
    }
}
