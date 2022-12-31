using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.ActionFilters;
using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Models.Response;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Validation;
using HomeAutio.Mqtt.GoogleHome.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAutio.Mqtt.GoogleHome.Controllers
{
    /// <summary>
    /// Devices controller.
    /// </summary>
    [Authorize]
    public class GoogleDeviceController : Controller
    {
        private readonly IGoogleDeviceRepository _deviceRepository;

        private readonly SyncIntentHandler _syncIntentHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleDeviceController"/> class.
        /// </summary>
        /// <param name="deviceRepository">Device repository.</param>
        /// <param name="syncIntentHandler">Sync intent handler.</param>
        public GoogleDeviceController(
            IGoogleDeviceRepository deviceRepository,
            SyncIntentHandler syncIntentHandler)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _syncIntentHandler = syncIntentHandler ?? throw new ArgumentNullException(nameof(syncIntentHandler));
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <returns>Response.</returns>
        public IActionResult Index()
        {
            var model = _deviceRepository.GetAll()
                .OrderBy(device => device.Id)
                .ToList();

            return View(model);
        }

        /// <summary>
        /// Gets a SYNC intent response for external validation.
        /// </summary>
        /// <returns>Response.</returns>
        public IActionResult SyncValidate()
        {
            var response = new Response
            {
                RequestId = Guid.NewGuid().ToString(),
                Payload = _syncIntentHandler.Handle(new SyncIntent())
            };

            return Json(response);
        }

        /// <summary>
        /// Gets a device json for external validation.
        /// </summary>
        /// <returns>Response.</returns>
        public IActionResult Json()
        {
            return Json(_deviceRepository.GetAll().ToDictionary(device => device.Id, device => device));
        }

        /// <summary>
        /// Create device.
        /// </summary>
        /// <returns>Response.</returns>
        [ImportModelState]
        public IActionResult Create()
        {
            var model = new DeviceViewModel
            {
                Id = string.Empty,
                Type = DeviceType.Unknown,
                WillReportState = true,
                Name = string.Empty
            };

            return View(model);
        }

        /// <summary>
        /// Create device.
        /// </summary>
        /// <param name="viewModel">View Model.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        [ExportModelState]
        public IActionResult Create(DeviceViewModel viewModel)
        {
            if (_deviceRepository.Contains(viewModel.Id))
            {
                ModelState.AddModelError("Id", "Device Id already exists");
            }

            // Name Info
            var nameInfo = new NameInfo
            {
                Name = viewModel.Name,
                DefaultNames = !string.IsNullOrEmpty(viewModel.DefaultNames)
                    ? viewModel.DefaultNames.Split(',').Select(x => x.Trim()).ToList()
                    : new List<string>(),
                Nicknames = !string.IsNullOrEmpty(viewModel.Nicknames)
                    ? viewModel.Nicknames.Split(',').Select(x => x.Trim()).ToList()
                    : new List<string>()
            };

            // Device Info
            DeviceInfo? deviceInfo = null;
            if (!string.IsNullOrEmpty(viewModel.Manufacturer) ||
                !string.IsNullOrEmpty(viewModel.Model) ||
                !string.IsNullOrEmpty(viewModel.HwVersion) ||
                !string.IsNullOrEmpty(viewModel.SwVersion))
            {
                deviceInfo = new DeviceInfo
                {
                    Manufacturer = !string.IsNullOrEmpty(viewModel.Manufacturer) ? viewModel.Manufacturer : null,
                    Model = !string.IsNullOrEmpty(viewModel.Model) ? viewModel.Model : null,
                    HwVersion = !string.IsNullOrEmpty(viewModel.HwVersion) ? viewModel.HwVersion : null,
                    SwVersion = !string.IsNullOrEmpty(viewModel.SwVersion) ? viewModel.SwVersion : null
                };
            }

            // Set new values
            var device = new Models.State.Device
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Disabled = viewModel.Disabled,
                WillReportState = viewModel.WillReportState,
                RoomHint = viewModel.RoomHint,
                Name = nameInfo,
                DeviceInfo = deviceInfo,
                Traits = new List<DeviceTrait>()
            };

            // Final validation
            foreach (var error in DeviceValidator.Validate(device))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }

            // Save changes
            _deviceRepository.Add(device);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete device.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        public IActionResult Delete([Required] string deviceId)
        {
            if (!_deviceRepository.Contains(deviceId))
            {
                return NotFound();
            }

            // Save changes
            _deviceRepository.Delete(deviceId);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edit device.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <returns>Response.</returns>
        [ImportModelState]
        public IActionResult Edit([Required] string deviceId)
        {
            var device = _deviceRepository.FindById(deviceId);
            if (device is null)
            {
                return NotFound();
            }

            var model = new DeviceViewModel
            {
                Id = device.Id,
                Type = device.Type,
                Disabled = device.Disabled,
                WillReportState = device.WillReportState,
                RoomHint = device.RoomHint,
                Name = device.Name.Name,
                DefaultNames = device.Name.DefaultNames is not null
                    ? string.Join(',', device.Name.DefaultNames)
                    : null,
                Nicknames = device.Name.Nicknames is not null
                    ? string.Join(',', device.Name.Nicknames)
                    : null,
                Manufacturer = device.DeviceInfo?.Manufacturer,
                Model = device.DeviceInfo?.Model,
                HwVersion = device.DeviceInfo?.HwVersion,
                SwVersion = device.DeviceInfo?.SwVersion,
                Traits = device.Traits != null
                    ? device.Traits.Select(x => x.Trait).OrderBy(trait => trait)
                    : new List<TraitType>()
            };

            return View(model);
        }

        /// <summary>
        /// Edit device.
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <param name="viewModel">View Model.</param>
        /// <returns>Response.</returns>
        [HttpPost]
        [ExportModelState]
        public IActionResult Edit([Required] string deviceId, DeviceViewModel viewModel)
        {
            var existingDevice = _deviceRepository.FindById(deviceId);
            if (existingDevice is null)
            {
                return NotFound();
            }

            // Name Info
            var nameInfo = new NameInfo
            {
                Name = viewModel.Name,
                DefaultNames = !string.IsNullOrEmpty(viewModel.DefaultNames)
                    ? viewModel.DefaultNames.Split(',').Select(x => x.Trim()).ToList()
                    : new List<string>(),
                Nicknames = !string.IsNullOrEmpty(viewModel.Nicknames)
                    ? viewModel.Nicknames.Split(',').Select(x => x.Trim()).ToList()
                    : new List<string>()
            };

            // Device Info
            DeviceInfo? deviceInfo = null;
            if (!string.IsNullOrEmpty(viewModel.Manufacturer) ||
                !string.IsNullOrEmpty(viewModel.Model) ||
                !string.IsNullOrEmpty(viewModel.HwVersion) ||
                !string.IsNullOrEmpty(viewModel.SwVersion))
            {
                deviceInfo = new DeviceInfo
                {
                    Manufacturer = !string.IsNullOrEmpty(viewModel.Manufacturer) ? viewModel.Manufacturer : null,
                    Model = !string.IsNullOrEmpty(viewModel.Model) ? viewModel.Model : null,
                    HwVersion = !string.IsNullOrEmpty(viewModel.HwVersion) ? viewModel.HwVersion : null,
                    SwVersion = !string.IsNullOrEmpty(viewModel.SwVersion) ? viewModel.SwVersion : null
                };
            }

            var device = new Models.State.Device
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Disabled = viewModel.Disabled,
                WillReportState = viewModel.WillReportState,
                RoomHint = viewModel.RoomHint,
                Name = nameInfo,
                DeviceInfo = deviceInfo,
                Traits = existingDevice.Traits
            };

            // Final validation
            foreach (var error in DeviceValidator.Validate(device))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { deviceId });
            }

            // Save changes
            _deviceRepository.Update(deviceId, device);

            return RedirectToAction("Index");
        }
    }
}
