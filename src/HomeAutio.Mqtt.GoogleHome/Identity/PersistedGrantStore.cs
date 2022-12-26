using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Persisted grant store.
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStoreWithExpiration
    {
        private static readonly object _readLock = new();
        private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        private readonly ILogger<PersistedGrantStore> _log;
        private readonly ConcurrentDictionary<string, PersistedGrant> _repository = new();
        private readonly string _file;
        private readonly int _refreshTokenGracePeriod;

        // Explicitly use the default contract resolver to force exact property serialization Base64 keys as they are case sensitive
        private readonly JsonSerializerSettings _jsonSerializerSettings = new() { ContractResolver = new DefaultContractResolver() };

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="configuration">Conffguration.</param>
        public PersistedGrantStore(ILogger<PersistedGrantStore> logger, IConfiguration configuration)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _file = configuration.GetRequiredValue<string>("oauth:tokenStoreFile");
            _refreshTokenGracePeriod = configuration.GetValue("oauth:refreshTokenGracePeriod", 0);
            RestoreFromFile();
        }

        /// <inheritdoc />
        public async Task StoreAsync(PersistedGrant grant)
        {
            _repository[grant.Key] = grant;

            await WriteToFileAsync();
        }

        /// <inheritdoc />
        public Task<PersistedGrant?> GetAsync(string key)
        {
            if (_repository.TryGetValue(key, out var token))
            {
                return Task.FromResult<PersistedGrant?>(token);
            }

            _log.LogWarning("Failed to find token with key {Key}", key);

            return Task.FromResult<PersistedGrant?>(null);
        }

        /// <inheritdoc />
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var query = _repository.AsEnumerable();

            if (!string.IsNullOrEmpty(filter.ClientId))
            {
                query = query.Where(x => x.Value.ClientId == filter.ClientId);
            }

            if (!string.IsNullOrEmpty(filter.SubjectId))
            {
                query = query.Where(x => x.Value.SubjectId == filter.SubjectId);
            }

            if (!string.IsNullOrEmpty(filter.SessionId))
            {
                query = query.Where(x => x.Value.SessionId == filter.SessionId);
            }

            var items = query.Select(x => x.Value).AsEnumerable();
            return Task.FromResult(items);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key)
        {
            if (_repository.TryRemove(key, out _))
            {
                await WriteToFileAsync();
            }
            else
            {
                _log.LogWarning("Failed to remove token with key {Key}", key);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            var query = _repository.AsEnumerable();

            if (!string.IsNullOrEmpty(filter.ClientId))
            {
                query = query.Where(x => x.Value.ClientId == filter.ClientId);
            }

            if (!string.IsNullOrEmpty(filter.SubjectId))
            {
                query = query.Where(x => x.Value.SubjectId == filter.SubjectId);
            }

            if (!string.IsNullOrEmpty(filter.SessionId))
            {
                query = query.Where(x => x.Value.SessionId == filter.SessionId);
            }

            var keys = query.Select(x => x.Key).ToArray();
            var numKeysRemoved = 0;
            foreach (var key in keys)
            {
                if (_repository.TryRemove(key, out _))
                {
                    numKeysRemoved++;
                }
                else
                {
                    _log.LogWarning("Failed to remove token with key {Key}", key);
                }
            }

            if (numKeysRemoved > 0)
            {
                await WriteToFileAsync();
            }
        }

        /// <inheritdoc />
        public async Task RemoveAllExpiredAsync()
        {
            var refreshTokenCutoff = DateTime.UtcNow.AddSeconds(_refreshTokenGracePeriod * -1);
            var query = _repository
                .Where(x => x.Value.Expiration < DateTime.UtcNow || (x.Value.ConsumedTime != null && x.Value.ConsumedTime.Value < refreshTokenCutoff));

            var keys = query.Select(x => x.Key).ToArray();
            var numKeysRemoved = 0;
            foreach (var key in keys)
            {
                if (_repository.TryRemove(key, out _))
                {
                    numKeysRemoved++;
                }
                else
                {
                    _log.LogWarning("Failed to remove token with key {Key}", key);
                }
            }

            if (numKeysRemoved > 0)
            {
                await WriteToFileAsync();
            }
        }

        /// <summary>
        /// Initialize current state from file.
        /// </summary>
        private void RestoreFromFile()
        {
            if (File.Exists(_file))
            {
                lock (_readLock)
                {
                    var fileContents = File.ReadAllText(_file);
                    if (string.IsNullOrEmpty(fileContents))
                    {
                        _log.LogWarning("Token file {File} already exists but is empty", _file);
                        return;
                    }

                    var deserializedFileContents = JsonConvert.DeserializeObject<Dictionary<string, PersistedGrant>>(fileContents, _jsonSerializerSettings);
                    if (deserializedFileContents is not null)
                    {
                        _repository.Clear();
                        foreach (var record in deserializedFileContents)
                        {
                            if (!_repository.TryAdd(record.Key, record.Value))
                            {
                                _log.LogWarning("Failed to restore token with key {Key}", record.Key);
                            }
                        }

                        _log.LogInformation("Restored tokens from {File}", _file);
                    }
                    else
                    {
                        throw new ArgumentException($"Token file {_file} contents invalid");
                    }
                }
            }
        }

        /// <summary>
        /// Write the current state to file.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task WriteToFileAsync()
        {
            _log.LogInformation("Writing tokens to {File}", _file);

            await _semaphoreSlim.WaitAsync();

            try
            {
                var contents = JsonConvert.SerializeObject(_repository, _jsonSerializerSettings);
                await File.WriteAllTextAsync(_file, contents);

                _log.LogInformation("Wrote tokens to {File}", _file);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
