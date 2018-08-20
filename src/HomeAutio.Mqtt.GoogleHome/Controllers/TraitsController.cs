using System.ComponentModel.DataAnnotations;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.ActionFilters;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.Controllers
{
    /// <summary>
    /// Traits controller.
    /// </summary>
    public class TraitsController : Controller
    {
        private readonly ILogger<TraitsController> _log;

        private readonly GoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraitsController"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="deviceRepository">Device repository.</param>
        public TraitsController(
            ILogger<TraitsController> logger,
            GoogleDeviceRepository deviceRepository)
        {
            _log = logger;
            _deviceRepository = deviceRepository;
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
                return NotFound();

            var model = new TraitViewModel();

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
                return NotFound();

            var device = _deviceRepository.Get(deviceId);
            if (device.Traits.Any(x => x.Trait == viewModel.Trait))
                ModelState.AddModelError("Trait", "Device already contains trait");

            if (!ModelState.IsValid)
                return RedirectToAction("Create", new { deviceId });

            // Set values
            var trait = new DeviceTrait
            {
                Trait = viewModel.Trait
            };

            device.Traits.Add(trait);

            // Save changes
            _deviceRepository.Persist();

            return RedirectToAction("Edit", "Devices", new { deviceId });
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
                return NotFound();

            var device = _deviceRepository.Get(deviceId);

            var traitEnumId = traitId.ToEnum<TraitType>();
            if (!device.Traits.Any(x => x.Trait == traitEnumId))
                return NotFound();

            device.Traits.Remove(device.Traits.First(x => x.Trait == traitEnumId));

            // Save changes
            _deviceRepository.Persist();

            return RedirectToAction("Edit", "Devices", new { deviceId });
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
            if (!_deviceRepository.Contains(deviceId))
                return NotFound();

            var device = _deviceRepository.Get(deviceId);

            var traitEnumId = traitId.ToEnum<TraitType>();
            if (!device.Traits.Any(x => x.Trait == traitEnumId))
                return NotFound();

            // Get trait
            var trait = device.Traits.First(x => x.Trait == traitEnumId);
            var model = new TraitViewModel
            {
                Trait = trait.Trait
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
            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { deviceId, traitId });

            if (!_deviceRepository.Contains(deviceId))
                return NotFound();

            var device = _deviceRepository.Get(deviceId);

            var traitEnumId = traitId.ToEnum<TraitType>();
            if (!device.Traits.Any(x => x.Trait == traitEnumId))
                return NotFound();

            // Lock the trait type just in case
            viewModel.Trait = traitEnumId;

            // Set new values
            var trait = device.Traits.FirstOrDefault(x => x.Trait == traitEnumId);

            // Save changes
            _deviceRepository.Persist();

            return RedirectToAction("Edit", "Devices", new { deviceId });
        }
    }
}
