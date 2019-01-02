using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.ActionFilters;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Validation;
using HomeAutio.Mqtt.GoogleHome.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Controllers
{
    /// <summary>
    /// Devices controller.
    /// </summary>
    [Authorize]
    public class DevicesController : Controller
    {
        private readonly ILogger<DevicesController> _log;

        private readonly IMessageHub _messageHub;
        private readonly GoogleDeviceRepository _deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesController"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="deviceRepository">Device repository.</param>
        public DevicesController(
            ILogger<DevicesController> logger,
            IMessageHub messageHub,
            GoogleDeviceRepository deviceRepository)
        {
            _log = logger;
            _messageHub = messageHub;
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <returns>Response.</returns>
        public IActionResult Index()
        {
            var model = _deviceRepository.GetAll();

            return View(model);
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
            var device = new Device
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Disabled = viewModel.Disabled,
                WillReportState = viewModel.WillReportState,
                RoomHint = viewModel.RoomHint,
                Name = new Models.NameInfo
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
                    device.DeviceInfo = new Models.DeviceInfo();

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
            _deviceRepository.Persist();

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

            // Determine subscription changes
            var device = _deviceRepository.Get(deviceId);
            var deletedTopics = device.Traits
                .SelectMany(trait => trait.State)
                .Where(state => !string.IsNullOrEmpty(state.Value.Topic))
                .Select(state => state.Value.Topic);

            // Save changes
            _deviceRepository.Delete(deviceId);
            _deviceRepository.Persist();

            if (!device.Disabled)
            {
                // Publish event for subscription changes
                _messageHub.Publish(new ConfigSubscriptionChangeEvent { DeletedSubscriptions = deletedTopics });
            }

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
                RoomHint = device.RoomHint,
                Type = device.Type,
                WillReportState = device.WillReportState,
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
                model.Traits = device.Traits.Select(x => x.Trait);
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

            var currentDevice = _deviceRepository.Get(deviceId);

            // Poor mans deep clone for validation check
            var device = JsonConvert.DeserializeObject<Device>(JsonConvert.SerializeObject(currentDevice));

            // Set new values
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
                    device.DeviceInfo = new Models.DeviceInfo();

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

            // Determine if subscription changes need to be published
            var disabledStateChanged = currentDevice.Disabled != device.Disabled;

            // Set values on current device
            currentDevice.Id = device.Id;
            currentDevice.Type = device.Type;
            currentDevice.Disabled = device.Disabled;
            currentDevice.WillReportState = device.WillReportState;
            currentDevice.RoomHint = device.RoomHint;
            currentDevice.Name = device.Name;
            currentDevice.DeviceInfo = device.DeviceInfo;

            // Handle device id change
            if (deviceId != currentDevice.Id)
            {
                _deviceRepository.ChangeDeviceId(deviceId, currentDevice.Id);
            }

            // Save changes
            _deviceRepository.Persist();

            if (disabledStateChanged)
            {
                // Publish necessary subscription changes
                var deviceTopics = currentDevice.Traits
                    .SelectMany(trait => trait.State)
                    .Where(state => !string.IsNullOrEmpty(state.Value.Topic))
                    .Select(state => state.Value.Topic);
                if (device.Disabled)
                {
                    // Publish event for subscription changes
                    _messageHub.Publish(new ConfigSubscriptionChangeEvent { DeletedSubscriptions = deviceTopics });
                }
                else
                {
                    // Publish event for subscription changes
                    _messageHub.Publish(new ConfigSubscriptionChangeEvent { AddedSubscriptions = deviceTopics });
                }
            }

            return RedirectToAction("Index");
        }
    }
}
