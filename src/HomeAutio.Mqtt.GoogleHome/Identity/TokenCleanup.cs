using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Token cleanup processor.
    /// </summary>
    public class TokenCleanup
    {
        private readonly ILogger<TokenCleanup> _log;
        private readonly TimeSpan _cleanupInterval;
        private readonly IPersistedGrantStoreWithExpiration _grantStore;

        private CancellationTokenSource? _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCleanup"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="grantStore">Grant store.</param>
        /// <param name="interval">Interval.</param>
        public TokenCleanup(ILogger<TokenCleanup> logger, IPersistedGrantStoreWithExpiration grantStore, int interval = 60)
        {
            if (interval < 1)
            {
                throw new ArgumentException("Token cleanup interval must be at least 1 second");
            }

            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _grantStore = grantStore ?? throw new ArgumentNullException(nameof(grantStore));
            _cleanupInterval = TimeSpan.FromSeconds(interval);
        }

        /// <summary>
        /// Starts the token cleanup polling.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public void Start(CancellationToken cancellationToken)
        {
            if (_source != null)
            {
                throw new InvalidOperationException("Token cleanup already started. Call Stop first.");
            }

            _log.LogDebug("Starting grant removal");

            _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Factory.StartNew(() => StartInternalAsync(_source.Token));
        }

        /// <summary>
        /// Stops the token cleanup polling.
        /// </summary>
        public void Stop()
        {
            if (_source == null)
            {
                throw new InvalidOperationException("Token cleanup not started. Call Start first.");
            }

            _log.LogDebug("Stopping grant removal");

            _source.Cancel();
            _source = null;
        }

        private async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _log.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(_cleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _log.LogDebug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    _log.LogError("Task.Delay exception: {Message}. Exiting.", ex.Message);
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _log.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                await RemoveExpiredGrantsAsync();
            }
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        private async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _log.LogInformation("Removing expired grants");

                await _grantStore.RemoveAllExpiredAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Exception removing expired grants: {Exception}", ex.Message);
            }
        }
    }
}
