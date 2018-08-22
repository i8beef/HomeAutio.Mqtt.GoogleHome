using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Persisted grant store with extra expiration methods.
    /// </summary>
    public interface IPersistedGrantStoreWithExpiration : IPersistedGrantStore
    {
        /// <summary>
        /// Removes expired grants.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task RemoveAllExpiredAsync();
    }
}
