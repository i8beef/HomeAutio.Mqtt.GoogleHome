using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.Extensions
{
    public class JsonSchemaExtensionsTests
    {
        [Theory]
        [InlineData(TraitType.OnOff, null, GoogleType.Unknown)]
        [InlineData(TraitType.OnOff, "on", GoogleType.Bool)]
        [InlineData(TraitType.OpenClose, "openPercent", GoogleType.Numeric)]
        [InlineData(TraitType.Modes, "currentModeSettings.test", GoogleType.String)]
        [InlineData(TraitType.ColorSetting, "color.spectrumHsv.saturation", GoogleType.Numeric)]
        [InlineData(TraitType.EnergyStorage, "capacityRemaining.[0].rawValue", GoogleType.Numeric)]
        [InlineData(TraitType.SensorState, "currentSensorStateData.[0].currentSensorState", GoogleType.String)]
        [InlineData(TraitType.SensorState, "currentSensorStateData.[0].rawValue", GoogleType.Numeric)]
        public void GetGoogleTypeForFlattenedPathReturns(TraitType traitType, string target, GoogleType googleType)
        {
            // Arrange
            var schemas = TraitSchemaProvider.GetTraitSchemas();
            var schema = schemas.FirstOrDefault(x => x.Trait == traitType);

            // Act
            var result = schema.StateSchema.Validator.GetGoogleTypeForFlattenedPath(target);

            // Assert
            Assert.Equal(googleType, result);
        }

        [Theory]
        [InlineData(TraitType.OnOff, "on")]
        [InlineData(TraitType.OpenClose, "openPercent")]
        [InlineData(TraitType.ColorSetting, "color.spectrumHsv.saturation")]
        [InlineData(TraitType.EnergyStorage, "capacityRemaining.[0].rawValue")]
        public void ValidateFlattenedPathReturnsTrue(TraitType traitType, string target)
        {
            // Arrange
            var schemas = TraitSchemaProvider.GetTraitSchemas();
            var schema = schemas.FirstOrDefault(x => x.Trait == traitType);

            // Act
            var result = schema.StateSchema.Validator.FlattenedPathExists(target);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(TraitType.OnOff, null)]
        [InlineData(TraitType.OnOff, "bad")]
        [InlineData(TraitType.ColorSetting, "color.spectrumHsv.bad")]
        [InlineData(TraitType.EnergyStorage, "capacityRemaining.[0].bad")]
        public void ValidateFlattenedPathReturnsFalse(TraitType traitType, string target)
        {
            // Arrange
            var schemas = TraitSchemaProvider.GetTraitSchemas();
            var schema = schemas.FirstOrDefault(x => x.Trait == traitType);

            // Act
            var result = schema.StateSchema.Validator.FlattenedPathExists(target);

            // Assert
            Assert.False(result);
        }
    }
}
