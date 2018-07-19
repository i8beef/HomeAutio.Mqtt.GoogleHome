using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Persisted grant store.
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly ConcurrentDictionary<string, PersistedGrant> _repository = new ConcurrentDictionary<string, PersistedGrant>();
        private readonly string _file;
        private object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="configuration">Conffguration.</param>
        public PersistedGrantStore(IConfiguration configuration)
        {
            _file = configuration.GetValue<string>("oauth:tokenStoreFile");
            RestoreFromFile();
        }

        /// <inheritdoc />
        public Task StoreAsync(PersistedGrant grant)
        {
            _repository[grant.Key] = grant;

            WriteToFile();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<PersistedGrant> GetAsync(string key)
        {
            PersistedGrant token;
            if (_repository.TryGetValue(key, out token))
            {
                return Task.FromResult(token);
            }

            return Task.FromResult<PersistedGrant>(null);
        }

        /// <inheritdoc />
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var query = _repository.
                Where(x => x.Value.SubjectId == subjectId)
                .Select(x => x.Value);

            var items = query.ToArray().AsEnumerable();
            return Task.FromResult(items);
        }

        /// <inheritdoc />
        public Task RemoveAsync(string key)
        {
            _repository.TryRemove(key, out _);

            WriteToFile();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var query =
                from item in _repository
                where item.Value.ClientId == clientId &&
                    item.Value.SubjectId == subjectId
                select item.Key;

            var keys = query.ToArray();
            foreach (var key in keys)
            {
                _repository.TryRemove(key, out _);
            }

            WriteToFile();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var query = _repository
                .Where(x => x.Value.SubjectId == subjectId && x.Value.ClientId == clientId && x.Value.Type == type)
                .Select(x => x.Key);

            var keys = query.ToArray();
            foreach (var key in keys)
            {
                _repository.TryRemove(key, out _);
            }

            WriteToFile();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Initialize current state from file.
        /// </summary>
        private void RestoreFromFile()
        {
            if (File.Exists(_file))
            {
                lock (_lock)
                {
                    var fileContents = File.ReadAllText(_file);
                    var deserializedFileContents = JsonConvert.DeserializeObject<Dictionary<string, PersistedGrant>>(fileContents);

                    _repository.Clear();
                    foreach (var record in deserializedFileContents)
                    {
                        _repository.TryAdd(record.Key, record.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Write the current state to file.
        /// </summary>
        private void WriteToFile()
        {
            lock (_lock)
            {
                var contents = JsonConvert.SerializeObject(_repository);
                File.WriteAllText(_file, contents);
            }
        }
    }
}
