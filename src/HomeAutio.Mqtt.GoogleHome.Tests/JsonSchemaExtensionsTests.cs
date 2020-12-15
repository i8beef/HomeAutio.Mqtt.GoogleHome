using System.Linq;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Validation;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests
{
    public class JsonSchemaExtensionsTests
    {
        [Theory]
        [InlineData(TraitType.OnOff, "on")]
        [InlineData(TraitType.ColorSetting, "color.spectrumHsv.saturation")]
        [InlineData(TraitType.EnergyStorage, "capacityRemaining.[0].rawValue")]
        public void ValidateFlattenedPathReturnsTrue(TraitType traitType, string target)
        {
            // Arrange
            var schemas = TraitSchemaProvider.GetTraitSchemas();
            var schema = schemas.FirstOrDefault(x => x.Trait == traitType);

            // Act
            var result = schema.StateSchema.Validator.ValidateFlattenedPath(target);

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
            var result = schema.StateSchema.Validator.ValidateFlattenedPath(target);

            // Assert
            Assert.False(result);
        }
    }
}
