using System.Collections.Generic;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void CanFlattenDictionary()
        {
            // Arrange
            var nestedDictionary = new Dictionary<string, object>
            {
                {
                    "color",
                    new Dictionary<string, object>
                    {
                        {
                            "spectrumHSV",
                            new Dictionary<string, object>
                            {
                                { "hue", 123 },
                                { "saturation", 456 },
                                { "value", 789 },
                            }
                        }
                    }
                }
            };

            // Act
            var result = nestedDictionary.ToFlatDictionary();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.True(result.ContainsKey("color.spectrumHSV.hue"));
            Assert.Equal(123, result["color.spectrumHSV.hue"]);
            Assert.True(result.ContainsKey("color.spectrumHSV.saturation"));
            Assert.Equal(456, result["color.spectrumHSV.saturation"]);
            Assert.True(result.ContainsKey("color.spectrumHSV.value"));
            Assert.Equal(789, result["color.spectrumHSV.value"]);
        }

        [Fact]
        public void CanNestDictionary()
        {
            // Arrange
            var flattenedDictionary = new Dictionary<string, object>
            {
                { "color.spectrumHSV.hue", 123 },
                { "color.spectrumHSV.saturation", 456 },
                { "color.spectrumHSV.value", 789 }
            };

            // Act
            var result = flattenedDictionary.ToNestedDictionary();

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
