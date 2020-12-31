using HomeAutio.Mqtt.GoogleHome.Extensions;
using System.Collections.Generic;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Extensions
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
                            "spectrumHsv",
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
            Assert.True(result.ContainsKey("color.spectrumHsv.hue"));
            Assert.Equal(123, result["color.spectrumHsv.hue"]);
            Assert.True(result.ContainsKey("color.spectrumHsv.saturation"));
            Assert.Equal(456, result["color.spectrumHsv.saturation"]);
            Assert.True(result.ContainsKey("color.spectrumHsv.value"));
            Assert.Equal(789, result["color.spectrumHsv.value"]);
        }

        [Fact]
        public void CanNestDictionary()
        {
            // Arrange
            var flattenedDictionary = new Dictionary<string, object>
            {
                { "color.spectrumHsv.hue", 123 },
                { "color.spectrumHsv.saturation", 456 },
                { "color.spectrumHsv.value", 789 }
            };

            // Act
            var result = flattenedDictionary.ToNestedDictionary();

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

        [Fact]
        public void CanConvertArrays()
        {
            // Arrange
            var flattenedDictionary = new Dictionary<string, object>
            {
                { "currentSensorStateData.[0].name", "name1" },
                { "currentSensorStateData.[0].currentSensorState", "sensorState1" },
                { "currentSensorStateData.[0].rawValue", 100.1 },
                { "currentSensorStateData.[1].name", "name2" },
                { "currentSensorStateData.[1].currentSensorState", "sensorState2" },
                { "currentSensorStateData.[1].rawValue", 200.1 },
                { "deepvalue.level2.[0].name", "name2" }
            };

            // Act
            var result = flattenedDictionary.ToNestedDictionary();

            // Assert
            Assert.True(result.ContainsKey("currentSensorStateData"));
            Assert.True(result.ContainsKey("deepvalue"));

            var currentSensorStateDataResult = (IList<object>)result["currentSensorStateData"];

            var firstResult = (IDictionary<string, object>)currentSensorStateDataResult[0];
            Assert.Equal(flattenedDictionary["currentSensorStateData.[0].name"], firstResult["name"]);
            Assert.Equal(flattenedDictionary["currentSensorStateData.[0].currentSensorState"], firstResult["currentSensorState"]);
            Assert.Equal(flattenedDictionary["currentSensorStateData.[0].rawValue"], firstResult["rawValue"]);

            var secondResult = (IDictionary<string, object>)currentSensorStateDataResult[1];
            Assert.Equal(flattenedDictionary["currentSensorStateData.[1].name"], secondResult["name"]);
            Assert.Equal(flattenedDictionary["currentSensorStateData.[1].currentSensorState"], secondResult["currentSensorState"]);
            Assert.Equal(flattenedDictionary["currentSensorStateData.[1].rawValue"], secondResult["rawValue"]);

            var deepvalueResult = (Dictionary<string, object>)result["deepvalue"];
            var level2Result = (IList<object>)deepvalueResult["level2"];
            var deepResult = (IDictionary<string, object>)level2Result[0];
            Assert.Equal(flattenedDictionary["deepvalue.level2.[0].name"], deepResult["name"]);
        }
    }
}
