using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using HomeAutio.Mqtt.GoogleHome.ActionFilters;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using HomeAutio.Mqtt.GoogleHome.JsonConverters;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Schema;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Models.State.Challenges;
using HomeAutio.Mqtt.GoogleHome.Validation;
using HomeAutio.Mqtt.GoogleHome.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Controllers
{
    /// <summary>
    /// Traits controller.
    /// </summary>
    [Authorize]
    public class GoogleTraitController : Controller
    {
        private readonly IGoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleTraitController"/> class.
        /// </summary>
        /// <param name="deviceRepository">Device repository.</param>
        public GoogleTraitController(IGoogleDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        }

        /// <summary>
        /// Edit device.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <returns>Response.</returns>
        [ImportModelState]
        public IActionResult Create([Required] string deviceId)
        {
            if (!_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            var model = new TraitViewModel
            {
                Trait = TraitType.Unknown
            };

            return View(model);
        }

        /// <summary>
        /// Edit trait.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <param name="viewModel">View Model.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        [ExportModelState]
        public IActionResult Create([Required] string deviceId, TraitViewModel viewModel)
        {
            if (!_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            var device = _deviceRepository.GetDetached(deviceId);
            if (device.Traits.Any(x => x.Trait == viewModel.Trait))
            {
                ModelState.AddModelError("Trait", "Device already contains trait");
            }

            // Set values
            var commands = !string.IsNullOrEmpty(viewModel.Commands) ? JsonConvert.DeserializeObject<Dictionary<string, IDictionary<string, string>>>(viewModel.Commands) : null;
            var trait = new DeviceTrait
            {
                Trait = viewModel.Trait,
                Attributes = !string.IsNullOrEmpty(viewModel.Attributes) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(viewModel.Attributes, new ObjectDictionaryConverter()) : null,
                Commands = commands ?? new Dictionary<string, IDictionary<string, string>>(),
                State = !string.IsNullOrEmpty(viewModel.State) ? JsonConvert.DeserializeObject<Dictionary<string, DeviceState>>(viewModel.State) : null,
                Challenge = viewModel.ChallengeType switch
                {
                    ChallengeType.Acknowledge => new AcknowledgeChallenge(),
                    ChallengeType.Pin => new PinChallenge { Pin = viewModel.ChallengePin ?? string.Empty },
                    ChallengeType.None => null,
                    _ => null
                }
            };

            // Final validation
            foreach (var error in DeviceTraitValidator.Validate(trait))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create", new { deviceId });
            }

            // Save changes
            device.Traits.Add(trait);
            _deviceRepository.Update(deviceId, device);

            return RedirectToAction("Edit", "GoogleDevice", new { deviceId });
        }

        /// <summary>
        /// Delete device.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <param name="traitId">Trait id.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        public IActionResult Delete([Required] string deviceId, [Required] string traitId)
        {
            if (!_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            var device = _deviceRepository.GetDetached(deviceId);

            var traitEnumId = traitId.ToEnum<TraitType>();
            if (!device.Traits.Any(x => x.Trait == traitEnumId))
            {
                return NotFound();
            }

            // Save changes
            device.Traits.Remove(device.Traits.First(x => x.Trait == traitEnumId));
            _deviceRepository.Update(deviceId, device);

            return RedirectToAction("Edit", "GoogleDevice", new { deviceId });
        }

        /// <summary>
        /// Edit device.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <param name="traitId">Trait id.</param>
        /// <returns>Response.</returns>
        [ImportModelState]
        public IActionResult Edit([Required] string deviceId, [Required] string traitId)
        {
            var device = _deviceRepository.FindById(deviceId);
            if (device is null)
            {
                return NotFound();
            }

            var traitEnumId = traitId.ToEnum<TraitType>();
            if (!device.Traits.Any(x => x.Trait == traitEnumId))
            {
                return NotFound();
            }

            // Get trait
            var trait = device.Traits.First(x => x.Trait == traitEnumId);
            var model = new TraitViewModel
            {
                Trait = trait.Trait,
                Attributes = trait.Attributes != null ? JsonConvert.SerializeObject(trait.Attributes, Formatting.Indented) : null,
                Commands = trait.Commands != null ? JsonConvert.SerializeObject(trait.Commands, Formatting.Indented) : null,
                State = trait.State != null ? JsonConvert.SerializeObject(trait.State, Formatting.Indented) : null,
                ChallengeType = trait.Challenge switch
                {
                    AcknowledgeChallenge => ChallengeType.Acknowledge,
                    PinChallenge => ChallengeType.Pin,
                    _ => ChallengeType.None
                },
                ChallengePin = trait.Challenge is PinChallenge pinChallenge ? pinChallenge.Pin : null
            };

            return View(model);
        }

        /// <summary>
        /// Edit trait.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <param name="traitId">Trait id.</param>
        /// <param name="viewModel">View Model.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        [ExportModelState]
        public IActionResult Edit([Required] string deviceId, [Required] string traitId, TraitViewModel viewModel)
        {
            if (!_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            var device = _deviceRepository.GetDetached(deviceId);

            var traitEnumId = traitId.ToEnum<TraitType>();
            if (!device.Traits.Any(x => x.Trait == traitEnumId))
            {
                return NotFound();
            }

            // Set new values
            var commands = !string.IsNullOrEmpty(viewModel.Commands) ? JsonConvert.DeserializeObject<Dictionary<string, IDictionary<string, string>>>(viewModel.Commands) : null;
            var trait = new DeviceTrait
            {
                Trait = traitEnumId,
                Attributes = !string.IsNullOrEmpty(viewModel.Attributes) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(viewModel.Attributes, new ObjectDictionaryConverter()) : null,
                Commands = commands ?? new Dictionary<string, IDictionary<string, string>>(),
                State = !string.IsNullOrEmpty(viewModel.State) ? JsonConvert.DeserializeObject<Dictionary<string, DeviceState>>(viewModel.State) : null,
                Challenge = viewModel.ChallengeType switch
                {
                    ChallengeType.Acknowledge => new AcknowledgeChallenge(),
                    ChallengeType.Pin => new PinChallenge { Pin = viewModel.ChallengePin ?? string.Empty },
                    ChallengeType.None => null,
                    _ => null
                }
            };

            // Replace trait
            device.Traits.Remove(device.Traits.First(x => x.Trait == traitEnumId));
            device.Traits.Add(trait);

            // Final validation
            foreach (var error in DeviceTraitValidator.Validate(trait))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { deviceId, traitId });
            }

            // Save changes
            _deviceRepository.Update(deviceId, device);

            return RedirectToAction("Edit", "GoogleDevice", new { deviceId });
        }

        /// <summary>
        /// Get trait examples.
        /// </summary>
        /// <param name="traitId">Trait id.</param>
        /// <returns>Response.</returns>
        public IActionResult Examples([Required] string traitId)
        {
            var schemas = TraitSchemaProvider.GetTraitSchemas();
            var targetSchema = schemas.FirstOrDefault(x => x.Trait == Enum.Parse<TraitType>(traitId));
            if (targetSchema == null)
            {
                return NotFound();
            }

            var stateExamples = new List<SchemaExample>();
            if (targetSchema.StateSchema != null)
            {
                stateExamples.AddRange(targetSchema.StateSchema.Examples);
            }

            // Flatten out command examples
            var commandExamples = new List<SchemaExample>();
            foreach (var commandSchema in targetSchema.CommandSchemas)
            {
                var commandName = commandSchema.Command.ToEnumString();

                // Add a "command delegation mode" example
                commandExamples.Add(new SchemaExample
                {
                    Comment = $"{commandName}<br/>Pure command delegation mode.",
                    Example = GetWrappedCommandExample(commandName)
                });

                foreach (var commandExample in commandSchema.Examples)
                {
                    // Transform on the way out to keep pure examples in schemas
                    commandExamples.Add(new SchemaExample
                    {
                        Comment = $"{commandName}<br/>{commandExample.Comment}",
                        Example = GetWrappedCommandExample(commandName, commandExample.Example)
                    });
                }

                // Add result examples to states
                if (commandSchema.ResultsExamples != null)
                {
                    stateExamples.AddRange(commandSchema.ResultsExamples);
                }
            }

            var examples = new
            {
                AttributeExamples = targetSchema.AttributeSchema?.Examples,
                StateExamples = stateExamples.Any() ? stateExamples : null,
                CommandExamples = commandExamples
            };

            return Json(examples);
        }

        /// <summary>
        /// Generates a pure command delegation mode example for the specified command.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="example">Example.</param>
        /// <returns>Command delegation example string.</returns>
        private static string GetWrappedCommandExample(string commandName, string? example = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine("{");
            sb.Append($"  \"{commandName}\": ");

            if (string.IsNullOrEmpty(example))
            {
                sb.AppendLine("null");
            }
            else
            {
                var exampleLines = example.Split(Environment.NewLine);
                for (var i = 0; i < exampleLines.Length; i++)
                {
                    // Handle indent modification
                    if (i != 0)
                    {
                        sb.Append("  ");
                    }

                    sb.AppendLine(exampleLines[i]);
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
