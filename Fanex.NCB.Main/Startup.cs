namespace Fanex.NCB.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.AspNetCore.Configuration;
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Client.Transport;
    using Fanex.NAP.Client.Web;
    using Fanex.NCB.Core.Models.DataDomain;
    using Fanex.NCB.Core.Services;
    using Fanex.NCB.CustomAspNetCoreModule;
    using Fanex.NCB.CustomAspNetCoreModule.Extensions;
    using Fanex.NCB.Main.AppConfig;
    using Fanex.NCB.Shared.MySQLExtension.DataDomain;
    using Fanex.NCB.Shared.MySQLExtension.Services;
    using Fanex.NCB.SkypeBot.App_Start;
    using HealthChecks.UI.Client;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddHealthChecks()
                    .AddUriAvailable(appSettings.AuthApi, "AuthApi")
                    .AddUriAvailable(appSettings.UserResourceApi, "UserResourceApi")
                    .AddUriAvailable(appSettings.TokenUrl, "TokenUrl")
                    .AddUriAvailable(appSettings.SkypeServiceUrl, "SkypeServiceUrl")

                    .AddMySql(Configuration.GetConnectionString("ChatBotDB"), "ChatBotDB", HealthStatus.Degraded,
              tags: new[] { "db", "sql", "myserver" });

            services.AddHealthChecksUI();

            Dictionary<string, ConnectionConfiguration> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key,
                              connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithMySql(resourcePath: "~/App_Data")
               .Run();

            services.AddSingleton<IConfiguration>(Configuration);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<List<DataConfig>>(Configuration.GetSection("SPName"));
            services.Configure<List<SkypeBotSetting>>(Configuration.GetSection("SkypeBot"));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Index";
                options.LogoutPath = "/Auth/SignOut";
                options.Cookie = new CookieBuilder
                {
                    Name = "Fanex.NCB.Auth",
                    Expiration = TimeSpan.FromMinutes(60),
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSession();
            services.AddSingleton<INapSettings>(new NapSettings(Configuration));
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IRestClient, RestClient>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBridgeClientService, BridgeClientService>();
            services.AddTransient<IDBConfigService, DBConfigService>();
            services.RegisterServices();
            services.RegisterRepositories();

            InitializeModules(services);
        }

        private static void InitializeModules(IServiceCollection services)
        {
            SkypeBotDependencyInitializer.Initialize(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseAspNetCoreExceptionHandler("NCB.Main");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=NewNotification}/{action=Index}/{id?}");
            });

            NapClientManager
               .UseConfig(app.ApplicationServices.GetService<INapSettings>())
               .UseRestClient(app.ApplicationServices.GetService<IRestClient>())
               .UseSession(app.ApplicationServices.GetService<INapSession>());

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI();//healthchecks-ui
        }
    }
}