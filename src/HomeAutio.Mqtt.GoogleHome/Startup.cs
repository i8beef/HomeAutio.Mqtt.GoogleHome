using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.App_Start;
using HomeAutio.Mqtt.GoogleHome.Identity;
using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.GoogleHomeGraph;
using HomeAutio.Mqtt.GoogleHome.Models.State;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            JsonSerializerConfig.Configure();

            // Http client factory registration.
            services.AddHttpClient();

            // Add message hub instance
            services.AddSingleton<IMessageHub, MessageHub>();

            // Device configuration from file
            services.AddSingleton<IGoogleDeviceRepository>(serviceProvider =>
            {
                var deviceConfigFile = Configuration.GetValue<string>("deviceConfigFile");
                return new GoogleDeviceRepository(
                    serviceProvider.GetRequiredService<ILogger<GoogleDeviceRepository>>(),
                    serviceProvider.GetRequiredService<IMessageHub>(),
                    deviceConfigFile);
            });

            // Build state cache from configuration
            services.AddSingleton(serviceProvider =>
            {
                var topics = serviceProvider.GetService<IGoogleDeviceRepository>().GetAll()
                    .Where(device => !device.Disabled)
                    .SelectMany(device => device.Traits)
                    .SelectMany(trait => trait.State.Values)
                    .Select(state => state.Topic)
                    .Distinct()
                    .Where(topic => topic != null);

                return new StateCache(topics.ToDictionary(x => x, x => string.Empty));
            });

            // Google Home Graph client
            services.AddSingleton<IHostedService, GoogleHomeGraphService>();
            services.AddSingleton(serviceProvider =>
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
                    serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(),
                    serviceAccount,
                    Configuration.GetValue<string>("googleHomeGraph:agentUserId"));
            });

            // Intent handlers
            services.AddTransient<SyncIntentHandler>();
            services.AddTransient<QueryIntentHandler>();
            services.AddTransient<ExecuteIntentHandler>();
            services.AddTransient<DisconnectIntentHandler>();

            // Setup MQTT hosted service
            services.AddSingleton<IHostedService, MqttService>(serviceProvider =>
            {
                var brokerSettings = new Core.BrokerSettings
                {
                    BrokerIp = Configuration.GetValue<string>("mqtt:brokerIp"),
                    BrokerPort = Configuration.GetValue<int>("mqtt:brokerPort"),
                    BrokerUsername = Configuration.GetValue<string>("mqtt:brokerUsername"),
                    BrokerPassword = Configuration.GetValue<string>("mqtt:brokerPassword"),
                    BrokerUseTls = Configuration.GetValue<bool>("mqtt:brokerUseTls", false)
                };

                // TLS settings
                if (brokerSettings.BrokerUseTls)
                {
                    var brokerTlsSettings = new Core.BrokerTlsSettings
                    {
                        AllowUntrustedCertificates = Configuration.GetValue<bool>("mqtt:brokerTlsSettings:allowUntrustedCertificates", false),
                        IgnoreCertificateChainErrors = Configuration.GetValue<bool>("mqtt:brokerTlsSettings:ignoreCertificateChainErrors", false),
                        IgnoreCertificateRevocationErrors = Configuration.GetValue<bool>("mqtt:brokerTlsSettings:ignoreCertificateRevocationErrors", false)
                    };

                    switch (Configuration.GetValue<string>("mqtt:brokerTlsSettings:protocol", "1.2"))
                    {
                        case "1.0":
                            brokerTlsSettings.SslProtocol = System.Security.Authentication.SslProtocols.Tls;
                            break;
                        case "1.1":
                            brokerTlsSettings.SslProtocol = System.Security.Authentication.SslProtocols.Tls11;
                            break;
                        case "1.2":
                        default:
                            brokerTlsSettings.SslProtocol = System.Security.Authentication.SslProtocols.Tls12;
                            break;
                    }

                    var brokerTlsCertificatesSection = Configuration.GetSection("mqtt:brokerTlsSettings:certificates");
                    brokerTlsSettings.Certificates = brokerTlsCertificatesSection.GetChildren()
                        .Select(x =>
                        {
                            var file = x.GetValue<string>("file");
                            var passPhrase = x.GetValue<string>("passPhrase");

                            if (!File.Exists(file))
                                throw new FileNotFoundException($"Broker Certificate '{file}' is missing!");

                            return !string.IsNullOrEmpty(passPhrase) ?
                                new X509Certificate2(file, passPhrase) :
                                new X509Certificate2(file);
                        }).ToList();

                    brokerSettings.BrokerTlsSettings = brokerTlsSettings;
                }

                return new MqttService(
                    serviceProvider.GetRequiredService<ILogger<MqttService>>(),
                    serviceProvider.GetRequiredService<IMessageHub>(),
                    brokerSettings,
                    serviceProvider.GetRequiredService<IGoogleDeviceRepository>(),
                    serviceProvider.GetRequiredService<StateCache>(),
                    Configuration.GetValue<string>("mqtt:topicRoot", "google/home"));
            });

            // Setup token cleanup hosted service
            services.AddSingleton<IHostedService, TokenCleanupService>();
            services.AddTransient<TokenCleanup>();

            // MVC
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                    opt.SerializerSettings.FloatParseHandling = FloatParseHandling.Decimal;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver { NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = false } };
                });

            // Proxy header forwarding
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // Identity Server 4
            services.AddSingleton<IPersistedGrantStoreWithExpiration, PersistedGrantStore>();
            services.AddSingleton<IPersistedGrantStore>(serviceProvider => serviceProvider.GetRequiredService<IPersistedGrantStoreWithExpiration>());
            services.AddTransient<IRefreshTokenService>(serviceProvider => new GracefulRefreshTokenService(
                serviceProvider.GetRequiredService<IRefreshTokenStore>(),
                serviceProvider.GetRequiredService<IProfileService>(),
                serviceProvider.GetRequiredService<ISystemClock>(),
                serviceProvider.GetRequiredService<ILogger<GracefulRefreshTokenService>>(),
                Configuration.GetValue("oauth:refreshTokenGracePeriod", 0)));

            var authority = Configuration.GetValue<string>("oauth:authority");
            var identityServerBuilder = services
                .AddIdentityServer(options =>
                {
                    options.IssuerUri = authority;
                })
                .AddInMemoryClients(Clients.Get(Configuration))
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiScopes(Resources.GetApiScopes())
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
                identityServerBuilder.AddDeveloperSigningCredential(true, "config/tempkey.jwk");
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
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = Configuration.GetValue<bool>("oauth:requireSSL");
                    options.Authority = authority;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Set the app base path when behind a proxy that changes it
            var pathBaseEnv = Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
            var pathBaseConfig = Configuration.GetValue<string>("appPathBase", null);
            var pathBase = pathBaseEnv ?? pathBaseConfig;
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            // Show error page in development mode
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Accept forward headers from proxies
            app.UseForwardedHeaders();

            // Add request response logging
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // Expose static file hosting
            app.UseStaticFiles();

            // MVC/API config
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=GoogleDevice}/{action=Index}/{id?}");
            });

            // Add identity server implementations
            app.UseIdentityServer();
        }
    }
}
