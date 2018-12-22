using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.Identity;
using HomeAutio.Mqtt.GoogleHome.Models;
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
using Newtonsoft.Json.Serialization;

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
            // Global JSON options
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                settings.Converters.Add(new StringEnumConverter());

                return settings;
            };

            // Http client factory registration.
            services.AddHttpClient();

            // Add message hub instance
            services.AddSingleton<IMessageHub>(serviceProvider => MessageHub.Instance);

            // Device configuration from file
            services.AddSingleton<GoogleDeviceRepository>(serviceProvider =>
            {
                var deviceConfigFile = Configuration.GetValue<string>("deviceConfigFile");
                return new GoogleDeviceRepository(
                    serviceProvider.GetRequiredService<ILogger<GoogleDeviceRepository>>(),
                    deviceConfigFile);
            });

            // Build state cache from configuration
            services.AddSingleton<StateCache>(serviceProvider =>
            {
                var stateValues = serviceProvider.GetService<GoogleDeviceRepository>().GetAll()
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
                    Configuration.GetValue<string>("googleHomeGraph:agentUserId"));
            });

            // Setup MQTT hosted service
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
                    serviceProvider.GetRequiredService<GoogleDeviceRepository>(),
                    serviceProvider.GetRequiredService<StateCache>(),
                    serviceProvider.GetRequiredService<IMessageHub>(),
                    serviceProvider.GetRequiredService<GoogleHomeGraphClient>(),
                    brokerSettings);
            });

            // Setup token cleanup hosted service
            services.AddSingleton<IHostedService, TokenCleanupService>();
            services.AddTransient<TokenCleanup>();

            // MVC
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Identity Server 4
            services.AddSingleton<IPersistedGrantStoreWithExpiration, PersistedGrantStore>();
            services.AddSingleton<IPersistedGrantStore>(x => x.GetRequiredService<IPersistedGrantStoreWithExpiration>());

            var publicOrigin = Configuration.GetValue<string>("oauth:publicOrigin");
            var authority = Configuration.GetValue<string>("oauth:authority");

            var identityServerBuilder = services
                .AddIdentityServer(options =>
                {
                    options.IssuerUri = authority;
                    options.PublicOrigin = publicOrigin;
                })
                .AddInMemoryClients(Clients.Get(Configuration))
                .AddInMemoryIdentityResources(Resources.GetIdentityResources(Configuration))
                .AddInMemoryApiResources(Resources.GetApiResources(Configuration))
                .AddTestUsers(Users.Get(Configuration));

            // Get signing certificates
            var signingCertsSection = Configuration.GetSection("oauth:signingCerts");
            var signingCerts = signingCertsSection.GetChildren()
                .Select(x => new SigningCertificate
                {
                    File = x.GetValue<string>("file"),
                    PassPhrase = x.GetValue<string>("passPhrase")
                }).ToList();

            if (signingCerts.Any())
            {
                // Add primary cert
                var primarySigningCert = signingCerts.First();
                if (!File.Exists(primarySigningCert.File))
                    throw new FileNotFoundException($"Signing Certificate '{primarySigningCert.File}' is missing!");

                var cert = !string.IsNullOrEmpty(primarySigningCert.PassPhrase) ?
                    new X509Certificate2(primarySigningCert.File, primarySigningCert.PassPhrase) :
                    new X509Certificate2(primarySigningCert.File);

                identityServerBuilder.AddSigningCredential(cert);

                // Add any verification certs
                for (var i = 1; i < signingCerts.Count(); i++)
                {
                    var oldSigningCert = signingCerts[i];
                    if (!File.Exists(oldSigningCert.File))
                        throw new FileNotFoundException($"Signing Certificate '{oldSigningCert.File}' is missing!");

                    var oldCert = !string.IsNullOrEmpty(oldSigningCert.PassPhrase) ?
                        new X509Certificate2(oldSigningCert.File, oldSigningCert.PassPhrase) :
                        new X509Certificate2(oldSigningCert.File);

                    identityServerBuilder.AddValidationKey(oldCert);
                }
            }
            else
            {
                identityServerBuilder.AddDeveloperSigningCredential(true, "config/tempkey.rsa");
            }

            // Turn on authorization via Cookie (signin, default) and Bearer (API)
            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                })
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authority;
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
            var pathBase = Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Devices}/{action=Index}/{id?}");
            });
            app.UseIdentityServer();
        }
    }
}
