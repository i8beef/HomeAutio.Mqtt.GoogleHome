using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.Models.State;

namespace HomeAutio.Mqtt.GoogleHome.Models.Schema
{
    /// <summary>
    /// Trait schema.
    /// </summary>
    public class TraitSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraitSchema"/> class.
        /// </summary>
        private TraitSchema() { }

        /// <summary>
        /// Attribute schema.
        /// </summary>
        public AttributeSchema AttributeSchema { get; private set; }

        /// <summary>
        /// Command schema JSONs.
        /// </summary>
        public IList<CommandSchema> CommandSchemas { get; private set; } = new List<CommandSchema>();

        /// <summary>
        /// State schema.
        /// </summary>
        public StateSchema StateSchema { get; private set; }

        /// <summary>
        /// Trait type.
        /// </summary>
        public TraitType Trait { get; private set; }

        /// <summary>
        /// Instantiates from embedded resources for specified trait type.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An instantiated <see cref="TraitSchema"/>.</returns>
        public static async Task<TraitSchema> ForTraitType(TraitType traitType, CancellationToken cancellationToken = default)
        {
            // Get resource paths
            var allTraitsResourceBase = "HomeAutio.Mqtt.GoogleHome.schema.traits";
            var traitId = traitType.ToEnumString();
            var traitName = traitId.Substring(traitId.LastIndexOf('.') + 1).ToLower();
            var traitResourceBase = $"{allTraitsResourceBase}.{traitName}";

            // Get resource names
            var assembly = typeof(TraitType).Assembly;
            var resources = assembly.GetManifestResourceNames();

            // If no resources match, dont instantiate
            if (!resources.Any(x => x.StartsWith(traitResourceBase)))
                return null;

            var traitSchema = new TraitSchema { Trait = traitType };

            // Attributes
            var attributeFile = $"{traitResourceBase}.{traitName}.attributes.schema.json";
            traitSchema.AttributeSchema = await AttributeSchema.FromJson(GetResourceString(attributeFile, resources), cancellationToken);

            // States
            var statesFile = $"{traitResourceBase}.{traitName}.states.schema.json";
            traitSchema.StateSchema = await StateSchema.FromJson(GetResourceString(statesFile, resources), cancellationToken);

            // Commands
            var commandParamFiles = resources.Where(x => x.StartsWith($"{traitResourceBase}") && x.EndsWith(".params.schema.json"));
            foreach (var commandParamFile in commandParamFiles)
            {
                var commandResourceBase = commandParamFile.Replace(".params.schema.json", string.Empty);
                var commandTypeName = commandResourceBase.Replace($"{traitResourceBase}.", string.Empty);
                var commandResultsFile = $"{commandResourceBase}.results.schema.json";
                var commandErrorFile = $"{commandResourceBase}.errors.schema.json";

                var commandType = Enum.Parse<CommandType>(commandTypeName, true);

                var commandSchema = await CommandSchema.FromJson(
                    commandType,
                    GetResourceString(commandParamFile, resources),
                    GetResourceString(commandResultsFile, resources),
                    GetResourceString(commandErrorFile, resources),
                    cancellationToken);

                if (commandSchema != null)
                {
                    traitSchema.CommandSchemas.Add(commandSchema);
                }
            }

            return traitSchema;
        }

        /// <summary>
        /// Gets the Google type for the specified path is valid for this schema.
        /// </summary>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>The <see cref="GoogleType"/> for the specified path.</returns>
        public GoogleType GetGoogleTypeForFlattenedPath(string flattenedPath)
        {
            if (StateSchema != null)
            {
                var result = StateSchema.Validator.GetGoogleTypeForFlattenedPath(flattenedPath);
                if (result != GoogleType.Unknown)
                {
                    return result;
                }
            }

            if (CommandSchemas != null && CommandSchemas.Any())
            {
                foreach (var schema in CommandSchemas.Where(x => x.ResultsValidator != null))
                {
                    var result = schema.ResultsValidator.GetGoogleTypeForFlattenedPath(flattenedPath);
                    if (result != GoogleType.Unknown)
                    {
                        return result;
                    }
                }
            }

            return GoogleType.Unknown;
        }

        /// <summary>
        /// Gets the enum values, if any, for the specified path for this schema.
        /// </summary>
        /// <param name="flattenedPath">Flattened state path.</param>
        /// <returns>The enum values, or null if not an enum.</returns>
        public ICollection<object> GetEnumValuesForFlattenedPath(string flattenedPath)
        {
            if (StateSchema != null)
            {
                var result = StateSchema.Validator.GetEnumValuesForFlattenedPath(flattenedPath);
                if (result != null)
                {
                    return result;
                }
            }

            if (CommandSchemas != null && CommandSchemas.Any())
            {
                foreach (var schema in CommandSchemas.Where(x => x.ResultsValidator != null))
                {
                    var result = schema.ResultsValidator.GetEnumValuesForFlattenedPath(flattenedPath);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets resource file contents as string.
        /// </summary>
        /// <param name="resourceName">Resource name to retrieve.</param>
        /// <param name="resources">Available resource files.</param>
        /// <returns>Resource file contents.</returns>
        private static string GetResourceString(string resourceName, IEnumerable<string> resources)
        {
            var assembly = typeof(TraitType).Assembly;
            if (resources.Contains(resourceName))
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }

            return null;
        }
    }
}
