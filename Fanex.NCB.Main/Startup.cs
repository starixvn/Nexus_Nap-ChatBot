namespace Fanex.NCB.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.AspNetCore.Configuration;
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Client.Transport;
    using Fanex.NAP.Client.Web;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Main.AppConfig;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Sentry;

    public class Startup
    {
        private const int CookieExpiration = 60;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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

            LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value),
                    FrontEndDsn = new Dsn(Configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
                })
            });

            services.AddSingleton(Configuration);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<BotSettings>(Configuration.GetSection("ChatBotConfigs"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Index";
                options.LogoutPath = "/Auth/SignOut";
                options.Cookie = new CookieBuilder
                {
                    Name = "Fanex.NCB.Auth",
                    Expiration = TimeSpan.FromMinutes(CookieExpiration),
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDistributedRedisCache(options => {
                options.Configuration = "10.100.10.199:6379";
                options.InstanceName = "Fanex.NCB.Auth";
            });

            services.AddSession(options => {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(CookieExpiration);
            });

            services.AddSingleton<INapSettings>(new NapSettings(Configuration));
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IRestClient, RestClient>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ILogger, Logger>();
            services.RegisterServices();
            services.RegisterRepositories();
        }

        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
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
                    template: "{controller=Auth}/{action=Index}/{id?}");
            });

            NapClientManager
               .UseConfig(app.ApplicationServices.GetService<INapSettings>())
               .UseRestClient(app.ApplicationServices.GetService<IRestClient>())
               .UseSession(app.ApplicationServices.GetService<INapSession>());
        }
    }
}