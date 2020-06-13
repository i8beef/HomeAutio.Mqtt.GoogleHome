using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HomeAutio.Mqtt.GoogleHome.ActionFilters;
using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using HomeAutio.Mqtt.GoogleHome.Models;
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
            _deviceRepository = deviceRepository ?? throw new ArgumentException(nameof(deviceRepository));
            _syncIntentHandler = syncIntentHandler ?? throw new ArgumentException(nameof(syncIntentHandler));
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
                Payload = _syncIntentHandler.Handle(new Models.Request.SyncIntent())
            };

            return Json(response);
        }

        /// <summary>
        /// Create device.
        /// </summary>
        /// <returns>Response.</returns>
        [ImportModelState]
        public IActionResult Create()
        {
            var model = new DeviceViewModel();

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
                ModelState.AddModelError("Id", "Device Id already exists");

            // Set new values
            var device = new Models.State.Device
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Disabled = viewModel.Disabled,
                WillReportState = viewModel.WillReportState,
                RoomHint = viewModel.RoomHint,
                Name = new NameInfo
                {
                    Name = viewModel.Name
                },
                Traits = new List<DeviceTrait>()
            };

            // Default names
            if (!string.IsNullOrEmpty(viewModel.DefaultNames))
                device.Name.DefaultNames = viewModel.DefaultNames.Split(',').Select(x => x.Trim()).ToList();
            else
                device.Name.DefaultNames = new List<string>();

            // Nicknames
            if (!string.IsNullOrEmpty(viewModel.Nicknames))
                device.Name.Nicknames = viewModel.Nicknames.Split(',').Select(x => x.Trim()).ToList();
            else
                device.Name.Nicknames = new List<string>();

            // Device Info
            if (!string.IsNullOrEmpty(viewModel.Manufacturer) ||
                !string.IsNullOrEmpty(viewModel.Model) ||
                !string.IsNullOrEmpty(viewModel.HwVersion) ||
                !string.IsNullOrEmpty(viewModel.SwVersion))
            {
                if (device.DeviceInfo == null)
                    device.DeviceInfo = new DeviceInfo();

                device.DeviceInfo.Manufacturer = !string.IsNullOrEmpty(viewModel.Manufacturer) ? viewModel.Manufacturer : null;
                device.DeviceInfo.Model = !string.IsNullOrEmpty(viewModel.Model) ? viewModel.Model : null;
                device.DeviceInfo.HwVersion = !string.IsNullOrEmpty(viewModel.HwVersion) ? viewModel.HwVersion : null;
                device.DeviceInfo.SwVersion = !string.IsNullOrEmpty(viewModel.SwVersion) ? viewModel.SwVersion : null;
            }
            else
            {
                device.DeviceInfo = null;
            }

            // Final validation
            foreach (var error in DeviceValidator.Validate(device))
                ModelState.AddModelError(string.Empty, error);

            if (!ModelState.IsValid)
                return RedirectToAction("Create");

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
                return NotFound();

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
            if (!_deviceRepository.Contains(deviceId))
                return NotFound();

            var device = _deviceRepository.Get(deviceId);
            var model = new DeviceViewModel
            {
                Id = device.Id,
                Type = device.Type,
                Disabled = device.Disabled,
                WillReportState = device.WillReportState,
                RoomHint = device.RoomHint,
                Name = device.Name.Name,
                DefaultNames = string.Join(',', device.Name.DefaultNames),
                Nicknames = string.Join(',', device.Name.Nicknames),
            };

            if (device.DeviceInfo != null)
            {
                model.Manufacturer = device.DeviceInfo.Manufacturer;
                model.Model = device.DeviceInfo.Model;
                model.HwVersion = device.DeviceInfo.HwVersion;
                model.SwVersion = device.DeviceInfo.SwVersion;
            }

            if (device.Traits != null)
            {
                model.Traits = device.Traits.Select(x => x.Trait).OrderBy(trait => trait);
            }
            else
            {
                model.Traits = new List<TraitType>();
            }

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
            if (!_deviceRepository.Contains(deviceId))
                return NotFound();

            // Set new values
            var device = _deviceRepository.GetDetached(deviceId);
            device.Id = viewModel.Id;
            device.Type = viewModel.Type;
            device.Disabled = viewModel.Disabled;
            device.WillReportState = viewModel.WillReportState;
            device.RoomHint = viewModel.RoomHint;
            device.Name.Name = viewModel.Name;

            // Default names
            if (!string.IsNullOrEmpty(viewModel.DefaultNames))
                device.Name.DefaultNames = viewModel.DefaultNames.Split(',').Select(x => x.Trim()).ToList();
            else
                device.Name.DefaultNames = new List<string>();

            // Nicknames
            if (!string.IsNullOrEmpty(viewModel.Nicknames))
                device.Name.Nicknames = viewModel.Nicknames.Split(',').Select(x => x.Trim()).ToList();
            else
                device.Name.Nicknames = new List<string>();

            // Device Info
            if (!string.IsNullOrEmpty(viewModel.Manufacturer) ||
                !string.IsNullOrEmpty(viewModel.Model) ||
                !string.IsNullOrEmpty(viewModel.HwVersion) ||
                !string.IsNullOrEmpty(viewModel.SwVersion))
            {
                if (device.DeviceInfo == null)
                    device.DeviceInfo = new DeviceInfo();

                device.DeviceInfo.Manufacturer = !string.IsNullOrEmpty(viewModel.Manufacturer) ? viewModel.Manufacturer : null;
                device.DeviceInfo.Model = !string.IsNullOrEmpty(viewModel.Model) ? viewModel.Model : null;
                device.DeviceInfo.HwVersion = !string.IsNullOrEmpty(viewModel.HwVersion) ? viewModel.HwVersion : null;
                device.DeviceInfo.SwVersion = !string.IsNullOrEmpty(viewModel.SwVersion) ? viewModel.SwVersion : null;
            }
            else
            {
                device.DeviceInfo = null;
            }

            // Final validation
            foreach (var error in DeviceValidator.Validate(device))
                ModelState.AddModelError(string.Empty, error);

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { deviceId });

            // Save changes
            _deviceRepository.Update(deviceId, device);

            return RedirectToAction("Index");
        }
    }
}
