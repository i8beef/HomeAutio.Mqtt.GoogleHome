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
                                "color.temperatureK",
                                new DeviceState
                                {
                                    Topic = "device/color/temperatureK",
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumRgb",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumRGB",
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumHsv.hue",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumHSV/hue",
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumHsv.saturation",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumHSV/saturation",
                                    ValueMap = null
                                }
                            },
                            {
                                "color.spectrumHsv.value",
                                new DeviceState
                                {
                                    Topic = "device/color/spectrumHSV/value",
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
            Assert.True(colorResult.ContainsKey("spectrumHsv"));

            var spectrumHSVResult = (IDictionary<string, object>)colorResult["spectrumHsv"];
            Assert.True(spectrumHSVResult.ContainsKey("hue"));
            Assert.Equal(123, spectrumHSVResult["hue"]);
            Assert.True(spectrumHSVResult.ContainsKey("saturation"));
            Assert.Equal(456, spectrumHSVResult["saturation"]);
            Assert.True(spectrumHSVResult.ContainsKey("value"));
            Assert.Equal(789, spectrumHSVResult["value"]);
        }
    }
}
