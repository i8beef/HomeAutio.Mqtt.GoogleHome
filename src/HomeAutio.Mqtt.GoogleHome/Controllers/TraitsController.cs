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
        /// <param name="traitId">Trait id.</param>
        /// <returns>Response.</returns>
        [ImportModelState]
        public IActionResult Edit(string deviceId, string traitId)
        {
            if (deviceId == null || !_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            var traitEnumId = traitId.ToEnum<TraitType>();

            var device = _deviceRepository.Get(deviceId);
            if (traitId == null || !device.Traits.Any(x => x.Trait == traitEnumId))
            {
                return NotFound();
            }

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
        public IActionResult Edit(string deviceId, string traitId, TraitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { deviceId, traitId });
            }

            if (deviceId == null || !_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            var traitEnumId = traitId.ToEnum<TraitType>();

            // Set new values
            var device = _deviceRepository.Get(deviceId);
            var trait = device.Traits.FirstOrDefault(x => x.Trait == traitEnumId);
            if (trait == null)
            {
                trait = new DeviceTrait();
                device.Traits.Add(trait);
            }

            trait.Trait = viewModel.Trait;

            // Save changes
            _deviceRepository.Persist();

            return RedirectToAction("Edit", "Devices", new { deviceId });
        }
    }
}
