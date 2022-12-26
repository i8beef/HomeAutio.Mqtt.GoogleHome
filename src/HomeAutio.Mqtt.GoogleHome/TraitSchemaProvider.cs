using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Schema;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Trait schema provider.
    /// </summary>
    public static class TraitSchemaProvider
    {
        private static IList<TraitSchema>? _traitSchemaCache;

        /// <summary>
        /// Get trait schemas for validation.
        /// </summary>
        /// <returns>A dictionary of trait schemas.</returns>
        public static IList<TraitSchema> GetTraitSchemas()
        {
            if (_traitSchemaCache is null)
            {
                _traitSchemaCache = InitTraitSchemas().GetAwaiter().GetResult();
            }

            return _traitSchemaCache;
        }

        /// <summary>
        /// Initializes trait schemas.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A dictionary of trait schemas.</returns>
        private static async Task<IList<TraitSchema>> InitTraitSchemas(CancellationToken cancellationToken = default)
        {
            var traitTypesNames = Enum.GetNames(typeof(TraitType));
            var traitTypes = traitTypesNames
                .Where(x => x.ToLower() != "unknown");

            var result = new List<TraitSchema>();
            foreach (var traitType in traitTypes)
            {
                var traitSchema = await TraitSchema.ForTraitType(Enum.Parse<TraitType>(traitType), cancellationToken);
                if (traitSchema != null)
                {
                    result.Add(traitSchema);
                }
            }

            return result;
        }
    }
}
