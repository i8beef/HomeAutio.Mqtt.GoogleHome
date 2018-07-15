using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>A configured <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
