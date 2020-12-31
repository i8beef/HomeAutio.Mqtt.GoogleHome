using System.Threading;
using System.Threading.Tasks;
using HomeAutio.Mqtt.GoogleHome.Identity;
using Microsoft.Extensions.Hosting;

namespace HomeAutio.Mqtt.GoogleHome.Services
{
    /// <summary>
    /// Token cleanup service.
    /// </summary>
    public class TokenCleanupService : IHostedService
    {
        private readonly TokenCleanup _tokenCleanup;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCleanupService"/> class.
        /// </summary>
        /// <param name="tokenCleanup">Token cleanup.</param>
        public TokenCleanupService(TokenCleanup tokenCleanup)
        {
            _tokenCleanup = tokenCleanup;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenCleanup.Start(cancellationToken);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenCleanup.Stop();

            return Task.CompletedTask;
        }
    }
}
