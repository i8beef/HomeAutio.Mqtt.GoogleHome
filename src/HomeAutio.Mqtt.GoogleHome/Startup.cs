using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add message hub instance
            services.AddSingleton<IMessageHub>(serviceProvider => MessageHub.Instance);

            // Device configuration from file
            services.AddSingleton<DeviceConfiguration>(serviceProvider =>
            {
                var deviceConfigurationString = File.ReadAllText(Configuration.GetValue<string>("deviceConfigFile"));
                var deviceConfiguration = JsonConvert.DeserializeObject<Dictionary<string, Device>>(deviceConfigurationString);
                return new DeviceConfiguration(deviceConfiguration);
            });

            // Build state cache from configuration
            services.AddSingleton<StateCache>(serviceProvider =>
            {
                var stateValues = serviceProvider.GetService<DeviceConfiguration>().Values
                    .SelectMany(x => x.Traits)
                    //.Where(x => x.Trait != "action.devices.traits.CameraStream") // Ignore the special little snowflake
                    .SelectMany(x => x.State.Values)
                    .Select(x => x.Topic)
                    .Where(x => x != null);

                // Flatten state to get real topicss
                var topics = stateValues.OfType<string>()
                    .Union(stateValues.OfType<IDictionary<string, object>>()
                        .SelectMany(x => x.Values)
                        .OfType<string>());
                return new StateCache(topics.ToDictionary(x => x, x => ""));
            });

            // Setup client
            services.AddSingleton<IHostedService, MqttService>(serviceProvider => new MqttService(
                    serviceProvider.GetRequiredService<Microsoft.Extensions.Hosting.IApplicationLifetime>(),
                    serviceProvider.GetRequiredService<ILogger<MqttService>>(),
                    serviceProvider.GetRequiredService<DeviceConfiguration>(),
                    serviceProvider.GetRequiredService<StateCache>(),
                    serviceProvider.GetRequiredService<IMessageHub>(),
                    Configuration.GetValue<string>("brokerIp"),
                    Configuration.GetValue<int>("brokerPort"),
                    Configuration.GetValue<string>("brokerUsername"),
                    Configuration.GetValue<string>("brokerPassword")));

            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
