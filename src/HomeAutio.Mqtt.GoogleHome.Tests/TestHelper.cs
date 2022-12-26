using Microsoft.Extensions.Configuration;

namespace HomeAutio.Mqtt.GoogleHome.Tests
{
    /// <summary>
    /// Test helper methods.
    /// </summary>
    public static class TestHelper
    {
        private static IConfigurationRoot? _configurationRoot;

        /// <summary>
        /// Gets configuration from appsettings.json.
        /// </summary>
        /// <returns>A <see cref="IConfigurationRoot"/>.</returns>
        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configurationRoot is null)
                {
                    _configurationRoot = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();
                }

                return _configurationRoot;
            }
        }
    }
}
