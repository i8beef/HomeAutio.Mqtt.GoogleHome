using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models.Events;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using HomeAutio.Mqtt.GoogleHome.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Google device repository.
    /// </summary>
    public class GoogleDeviceRepository : IGoogleDeviceRepository
    {
        private readonly ILogger<GoogleDeviceRepository> _logger;
        private readonly string _deviceConfigFile;
        private readonly IMessageHub _messageHub;
        private readonly object _writeLock = new();

        private ConcurrentDictionary<string, Device> _devices;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleDeviceRepository"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="messageHub">Message nhub.</param>
        /// <param name="path">Device config file path.</param>
        public GoogleDeviceRepository(
            ILogger<GoogleDeviceRepository> logger,
            IMessageHub messageHub,
            string path)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHub = messageHub ?? throw new ArgumentNullException(nameof(messageHub));
            _deviceConfigFile = path;

            // Makes null checkers happy
            _devices = new ConcurrentDictionary<string, Device>();
            Refresh();
        }

        /// <inheritdoc />
        public void Add(Device device)
        {
            if (_devices.TryAdd(device.Id, device))
            {
                // Save changes
                Persist();

                // Determine if subscription changes need to be published
                if (!device.Disabled)
                {
                    // Publish necessary subscription changes
                    var deviceTopics = device.Traits
                        .Where(trait => trait.State is not null)
                        .SelectMany(trait => trait.State!)
                        .Where(state => !string.IsNullOrEmpty(state.Value.Topic))
                        .Select(state => state.Value.Topic!);

                    // Publish event for subscription changes
                    _messageHub.Publish(new ConfigSubscriptionChangeEvent { AddedSubscriptions = deviceTopics });
                }
            }
        }

        /// <inheritdoc />
        public bool Contains(string deviceId)
        {
            return _devices.ContainsKey(deviceId);
        }

        /// <inheritdoc />
        public void Delete(string deviceId)
        {
            if (_devices.TryRemove(deviceId, out var device))
            {
                // Save changes
                Persist();

                if (!device.Disabled)
                {
                    var deletedTopics = device.Traits
                        .Where(trait => trait.State is not null)
                        .SelectMany(trait => trait.State!)
                        .Where(state => !string.IsNullOrEmpty(state.Value.Topic))
                        .Select(state => state.Value.Topic!);

                    // Publish event for subscription changes
                    _messageHub.Publish(new ConfigSubscriptionChangeEvent { DeletedSubscriptions = deletedTopics });
                }
            }
        }

        /// <inheritdoc />
        public Device? FindById(string deviceId)
        {
            if (_devices.TryGetValue(deviceId, out var value))
            {
                return value;
            }

            return null;
        }

        /// <inheritdoc />
        public IList<Device> GetAll()
        {
            return _devices.Values.ToList();
        }

        /// <inheritdoc />
        public Device GetDetached(string deviceId)
        {
            var device = FindById(deviceId) ?? throw new InvalidOperationException($"Device id {deviceId} not found");

            return JsonConvert.DeserializeObject<Device>(JsonConvert.SerializeObject(device))!;
        }

        /// <inheritdoc />
        public void Refresh()
        {
            lock (_writeLock)
            {
                if (File.Exists(_deviceConfigFile))
                {
                    var deviceConfigurationString = File.ReadAllText(_deviceConfigFile);
                    var deviceConfiguration = JsonConvert.DeserializeObject<Dictionary<string, Device>>(deviceConfigurationString);
                    _devices = deviceConfiguration is not null
                        ? new ConcurrentDictionary<string, Device>(deviceConfiguration)
                        : new ConcurrentDictionary<string, Device>();

                    // Validate the config
                    foreach (var device in _devices)
                    {
                        var errors = DeviceValidator.Validate(device.Value);
                        if (errors.Any())
                        {
                            _logger.LogWarning("GoogleDevices.json issues detected for device {Device}: {DeviceConfigIssues}", device.Key, errors);
                        }
                    }

                    _logger.LogInformation($"Loaded device configuration from {_deviceConfigFile}");
                }
                else
                {
                    _devices = new ConcurrentDictionary<string, Device>();
                    _logger.LogInformation($"Device configuration file {_deviceConfigFile} not found, starting with empty set");
                }
            }
        }

        /// <inheritdoc />
        public void Update(string deviceId, Device device)
        {
            if (Contains(deviceId))
            {
                // Handle device id change
                if (deviceId != device.Id)
                {
                    ChangeDeviceId(deviceId, device.Id);
                }

                var currentDevice = FindById(device.Id);
                if (currentDevice != null)
                {
                    // Only publish subscriptions if disabled state changes
                    var disabledStateChanged = currentDevice.Disabled != device.Disabled;

                    // Capture topics
                    var existingTopics = currentDevice.Traits
                        .Where(trait => trait.State is not null)
                        .SelectMany(trait => trait.State!)
                        .Where(state => !string.IsNullOrEmpty(state.Value.Topic))
                        .Select(state => state.Value.Topic!);
                    var newTopics = device.Traits
                        .Where(trait => trait.State is not null)
                        .SelectMany(trait => trait.State!)
                        .Where(state => !string.IsNullOrEmpty(state.Value.Topic))
                        .Select(state => state.Value.Topic!);

                    // Save changes
                    if (_devices.TryUpdate(device.Id, device, currentDevice))
                    {
                        Persist();

                        // Publish event for subscription changes
                        if (disabledStateChanged)
                        {
                            if (device.Disabled)
                            {
                                // Enabled to disabled = unsubscribe to all old topics
                                _messageHub.Publish(new ConfigSubscriptionChangeEvent { DeletedSubscriptions = existingTopics });
                            }
                            else
                            {
                                // Disabled to enabled = subscribe to all current topics
                                _messageHub.Publish(new ConfigSubscriptionChangeEvent { AddedSubscriptions = newTopics });
                            }
                        }
                        else
                        {
                            if (!device.Disabled)
                            {
                                // Publish changed subscriptions
                                var addedTopics = newTopics.Where(topic => !existingTopics.Contains(topic));
                                var deletedTopics = existingTopics.Where(topic => !newTopics.Contains(topic));
                                _messageHub.Publish(new ConfigSubscriptionChangeEvent { AddedSubscriptions = addedTopics, DeletedSubscriptions = deletedTopics });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Changes the device ID for a device.
        /// </summary>
        /// <param name="originalId">Original ID.</param>
        /// <param name="newId">New ID.</param>
        private void ChangeDeviceId(string originalId, string newId)
        {
            var device = FindById(originalId);
            if (device != null)
            {
                // Ensure internal ID changed and matches.
                if (device.Id != newId)
                {
                    var newDevice = new Device
                    {
                        Id = newId,
                        Type = device.Type,
                        Name = device.Name,
                        Disabled = device.Disabled,
                        RoomHint = device.RoomHint,
                        WillReportState = device.WillReportState,
                        CustomData = device.CustomData,
                        DeviceInfo = device.DeviceInfo,
                        Traits = device.Traits
                    };

                    if (_devices.TryAdd(newId, newDevice))
                    {
                        if (_devices.TryRemove(originalId, out var _))
                        {
                            Persist();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Persists current device confiuguration.
        /// </summary>
        private void Persist()
        {
            lock (_writeLock)
            {
                var deviceString = JsonConvert.SerializeObject(_devices, Formatting.Indented);
                File.WriteAllText(_deviceConfigFile, deviceString);
                _logger.LogInformation($"Persisting device configuration changes to {_deviceConfigFile}");
            }

            // Publish an event to trigger REQUEST_SYNC
            _messageHub.Publish(new RequestSyncEvent());
        }
    }
}
