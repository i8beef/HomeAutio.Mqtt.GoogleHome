using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// Trait validation definition.
    /// </summary>
    public class TraitValidationDefinition
    {
        /// <summary>
        /// Attributes schema.
        /// </summary>
        public NJsonSchema.JsonSchema AttributesSchema { get; set; }

        /// <summary>
        /// States schema.
        /// </summary>
        public NJsonSchema.JsonSchema StatesSchema { get; set; }

        /// <summary>
        /// Comands schemas.
        /// </summary>
        public IDictionary<string, NJsonSchema.JsonSchema> CommandSchemas { get; set; }
    }
}
