using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace HomeAutio.Mqtt.GoogleHome.Identity
{
    /// <summary>
    /// Refresh token service with a grace period.
    /// </summary>
    public class GracefulRefreshTokenService : DefaultRefreshTokenService
    {
        private readonly int _refreshTokenGracePeriod;

        /// <summary>
        /// Initializes a new instance of the <see cref="GracefulRefreshTokenService" /> class.
        /// </summary>
        /// <param name="refreshTokenStore">The refresh token store.</param>
        /// <param name="profile">The profile.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="refreshTokenGracePeriod">RefreshToken grace period.</param>
        public GracefulRefreshTokenService(
            IRefreshTokenStore refreshTokenStore,
            IProfileService profile,
            ISystemClock clock,
            ILogger<GracefulRefreshTokenService> logger,
            int refreshTokenGracePeriod)
            : base(refreshTokenStore, profile, clock, logger)
        {
            _refreshTokenGracePeriod = refreshTokenGracePeriod;
        }

        /// <inheritdoc />
        protected override Task<bool> AcceptConsumedTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken.ConsumedTime.HasValue)
            {
                if (refreshToken.ConsumedTime.Value < Clock.UtcNow.AddSeconds(_refreshTokenGracePeriod * -1))
                {
                    return Task.FromResult(true);
                }
            }

            return base.AcceptConsumedTokenAsync(refreshToken);
        }
    }
}
