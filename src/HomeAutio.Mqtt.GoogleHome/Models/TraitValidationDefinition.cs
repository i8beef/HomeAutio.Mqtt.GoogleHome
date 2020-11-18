using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Trait validation definition.
    /// </summary>
    public class TraitValidationDefinition
    {
        /// <summary>
        /// Attribute examples.
        /// </summary>
        public IList<TraitExample> AttributeExamples { get; set; }

        /// <summary>
        /// Attribute schema.
        /// </summary>
        public string AttributeSchema { get; set; }

        /// <summary>
        /// Attributes schema.
        /// </summary>
        public NJsonSchema.JsonSchema AttributeValidator { get; set; }

        /// <summary>
        /// Command examples.
        /// </summary>
        public IDictionary<string, IList<TraitExample>> CommandExamples { get; set; }

        /// <summary>
        /// Attribute schema.
        /// </summary>
        public IDictionary<string, string> CommandSchemas { get; set; }

        /// <summary>
        /// Comands schemas.
        /// </summary>
        public IDictionary<string, NJsonSchema.JsonSchema> CommandValidators { get; set; }

        /// <summary>
        /// State schema.
        /// </summary>
        public string StateSchema { get; set; }

        /// <summary>
        /// State examples.
        /// </summary>
        public IList<TraitExample> StateExamples { get; set; }

        /// <summary>
        /// States schema.
        /// </summary>
        public NJsonSchema.JsonSchema StateValidator { get; set; }
    }
}
