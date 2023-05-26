using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
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
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, $"appsettings.{environmentName}.json"), optional: true)
                .Build();

            // Setup logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Logger.Information($"Loaded with configuration from: appsettings.json, {Path.Combine(Environment.CurrentDirectory, $"appsettings.{environmentName}.json")}");

            // Turn on or off PII data from Microsoft Identity stuff
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = config.GetValue("logPII", false);

            try
            {
                // Create host builder
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                // Configure host
                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);
                var app = builder.Build();
                startup.Configure(app, builder.Environment);

                // Run
                app.Run();
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
    }
}
