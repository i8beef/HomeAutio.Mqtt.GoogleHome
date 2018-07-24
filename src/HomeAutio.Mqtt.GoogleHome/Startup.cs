using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Identity;
using HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeAutio.Mqtt.GoogleHome
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Http client factory registration.
            services.AddHttpClient();

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
                    .SelectMany(x => x.State.Values)
                    .Select(x => x.Topic)
                    .Where(x => x != null);

                // Flatten state to get real topicss
                var topics = stateValues.OfType<string>()
                    .Union(stateValues.OfType<IDictionary<string, object>>()
                        .SelectMany(x => x.Values)
                        .OfType<string>());

                return new StateCache(topics.ToDictionary(x => x, x => string.Empty));
            });

            // Google Home Graph client
            services.AddSingleton<GoogleHomeGraphClient>(serviceProvider =>
            {
                ServiceAccount serviceAccount = null;
                var googleHomeServiceAccountFile = Configuration.GetValue<string>("googleHomeGraph:serviceAccountFile");
                if (!string.IsNullOrEmpty(googleHomeServiceAccountFile) && File.Exists(googleHomeServiceAccountFile))
                {
                    var googleHomeServiceAccountFileContents = File.ReadAllText(googleHomeServiceAccountFile);
                    serviceAccount = JsonConvert.DeserializeObject<ServiceAccount>(googleHomeServiceAccountFileContents);
                }

                return new GoogleHomeGraphClient(
                    serviceProvider.GetRequiredService<ILogger<GoogleHomeGraphClient>>(),
                    serviceProvider.GetRequiredService<IHttpClientFactory>(),
                    serviceAccount,
                    Configuration.GetValue<string>("googleHomeGraph:agentUserId"),
                    Configuration.GetValue<string>("googleHomeGraph:apiKey"));
            });

            // Setup client
            services.AddSingleton<IHostedService, MqttService>(serviceProvider =>
            {
                var brokerSettings = new Core.BrokerSettings
                {
                    BrokerIp = Configuration.GetValue<string>("mqtt:brokerIp"),
                    BrokerPort = Configuration.GetValue<int>("mqtt:brokerPort"),
                    BrokerUsername = Configuration.GetValue<string>("mqtt:brokerUsername"),
                    BrokerPassword = Configuration.GetValue<string>("mqtt:brokerPassword")
                };

                return new MqttService(
                    serviceProvider.GetRequiredService<ILogger<MqttService>>(),
                    serviceProvider.GetRequiredService<DeviceConfiguration>(),
                    serviceProvider.GetRequiredService<StateCache>(),
                    serviceProvider.GetRequiredService<IMessageHub>(),
                    serviceProvider.GetRequiredService<GoogleHomeGraphClient>(),
                    brokerSettings);
            });

            // MVC
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Identity Server 4
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var signingCertFile = Configuration.GetValue<string>("oauth:signingCert:file");
            var signingCertPassPhrase = Configuration.GetValue<string>("oauth:signingCert:passPhrase");
            if (!string.IsNullOrEmpty(signingCertFile))
            {
                if (!File.Exists(signingCertFile))
                {
                    throw new FileNotFoundException("Signing Certificate is missing!");
                }

                var cert = signingCertPassPhrase != null ?
                    new X509Certificate2(signingCertFile, signingCertPassPhrase) :
                    new X509Certificate2(signingCertFile);

                services.AddIdentityServer(options => options.IssuerUri = Configuration.GetValue<string>("oauth:authority"))
                    .AddSigningCredential(cert)
                    .AddInMemoryClients(Clients.Get(Configuration))
                    .AddInMemoryIdentityResources(Resources.GetIdentityResources(Configuration))
                    .AddInMemoryApiResources(Resources.GetApiResources(Configuration))
                    .AddTestUsers(Users.Get(Configuration));
            }
            else
            {
                services.AddIdentityServer(options => options.IssuerUri = Configuration.GetValue<string>("oauth:authority"))
                    .AddDeveloperSigningCredential(true, "config/tempkey.rsa")
                    .AddInMemoryClients(Clients.Get(Configuration))
                    .AddInMemoryIdentityResources(Resources.GetIdentityResources(Configuration))
                    .AddInMemoryApiResources(Resources.GetApiResources(Configuration))
                    .AddTestUsers(Users.Get(Configuration));
            }

            // Turn on authorization via Cookie (signin, default) and Bearer (API)
            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                }).AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration.GetValue<string>("oauth:authority");
                    options.ApiName = Configuration.GetValue<string>("oauth:resources:0:resourceName");
                    options.RequireHttpsMetadata = Configuration.GetValue<bool>("oauth:requireSSL");
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The hosting environment.</param>
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseIdentityServer();
        }
    }
}
