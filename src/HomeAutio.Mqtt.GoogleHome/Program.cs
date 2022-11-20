using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Main program entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = "Development";
            }

            // Setup config
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "config", $"appsettings.{environmentName}.json"), optional: true)
                .Build();

            // Setup logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Logger.Information($"Loaded with configuration from: appsettings.json, {Path.Combine(Environment.CurrentDirectory, "config", $"appsettings.{environmentName}.json")}");

            // Turn on or off PII data from Microsoft Identity stuff
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = config.GetValue("logPII", false);

            try
            {
                CreateWebHostBuilder(config, args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, ex.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>.
        /// </summary>
        /// <param name="config">Configuration.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>A configured <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(IConfigurationRoot config, string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder => configBuilder.AddConfiguration(config))
                .UseStartup<Startup>()
                .UseSerilog();
        }
    }
}
