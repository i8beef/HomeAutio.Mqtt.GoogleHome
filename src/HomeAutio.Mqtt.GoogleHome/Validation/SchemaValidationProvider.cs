using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Models;

namespace HomeAutio.Mqtt.GoogleHome.Validation
{
    /// <summary>
    /// Schema validation provider.
    /// </summary>
    public static class SchemaValidationProvider
    {
        /// <summary>
        /// Get trait schemas for validation.
        /// </summary>
        /// <returns>A dictionary of trait schemas.</returns>
        public static async Task<IDictionary<string, TraitValidationDefinition>> GetTraitSchemas()
        {
            var traitTypesNames = Enum.GetNames(typeof(TraitType));
            var traitTypes = traitTypesNames
                .Where(x => x.ToLower() != "unknown")
                .Select(x => Enum.Parse<TraitType>(x).ToEnumString());

            var commandNames = Enum.GetNames(typeof(CommandType));
            var commands = commandNames
                .Where(x => x.ToLower() != "unknown")
                .Select(x => Enum.Parse<CommandType>(x).ToEnumString());

            var result = new Dictionary<string, TraitValidationDefinition>();
            foreach (var trait in traitTypes)
            {
                var traitName = trait.Substring(trait.LastIndexOf('.') + 1).ToLower();
                var directory = Path.Join(@"D:\Projects\google-smarthome-validator\ConsoleApp1\ConsoleApp1\schema\traits", traitName);
                if (Directory.Exists(directory))
                {
                    var attributeFile = Path.Join(directory, $"{traitName}.attributes.schema.json");
                    var statesFile = Path.Join(directory, $"{traitName}.states.schema.json");

                    var commandSchemas = new Dictionary<string, NJsonSchema.JsonSchema>();
                    foreach (var commandFile in Directory.GetFiles(directory, "*.params.schema.json"))
                    {
                        var commandName = Path.GetFileName(commandFile).Replace(".params.schema.json", string.Empty);
                        var fileContents = File.ReadAllText(commandFile);

                        var command = commands.FirstOrDefault(x => x.Substring(x.LastIndexOf('.') + 1).ToLower() == commandName);

                        // Note: Temporary
                        if (command != null)
                        {
                            commandSchemas.Add(command, await NJsonSchema.JsonSchema.FromJsonAsync(fileContents));
                        }
                    }

                    string attributeSchema = null;
                    if (File.Exists(attributeFile))
                    {
                        attributeSchema = File.ReadAllText(attributeFile);
                    }

                    string statesSchema = null;
                    if (File.Exists(statesFile))
                    {
                        statesSchema = File.ReadAllText(statesFile);
                    }

                    var schemas = new TraitValidationDefinition
                    {
                        AttributesSchema = attributeSchema != null ? await NJsonSchema.JsonSchema.FromJsonAsync(attributeSchema) : null,
                        StatesSchema = statesSchema != null ? await NJsonSchema.JsonSchema.FromJsonAsync(statesSchema) : null,
                        CommandSchemas = commandSchemas
                    };

                    result.Add(trait, schemas);
                }
            }

            return result;
        }
    }
}
