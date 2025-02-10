using Fanex.Logging.Sentry.RavenClient;
using ONEbook.UM.DependencyInjection;
using SharpRaven;

namespace Fanex.NCB.Main
{
    using Data;
    using Fanex.NCB.Main.AppConfig;
    using Hubs;
    using Logging;
    using Logging.Extensions;
    using Logging.Extensions.AspNetCore;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Connections;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using NAP.Client;
    using NAP.Client.Configuration;
    using NAP.Client.Helpers.Web;
    using NAPResolver;
    using Shared.Middlewares;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage(Justification = "Startup code inject services")]
    public class Startup
    {
        private const int NotFoundStatus = 404;
        private const string SignalRPrefix = "ChatBotForLicensee:SignalR";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFanexData(Configuration);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

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

            ExceptionHandler
                .UseConfig(new LoggingConfiguration());

            services.AddSingleton(Configuration);
            services.RegisterConfigs(Configuration);

            RegisterAuthenticationCookie(services, Configuration);

            services.AddMvc(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); });

            var redisConfig = Configuration.GetSection("AuthSettings:RedisConfig").Value;
            var redis = ConnectionMultiplexer.Connect(redisConfig);
            services.AddDataProtection()
                .SetApplicationName("ChatBotForLicensee")
                .PersistKeysToStackExchangeRedis(redis, "ChatBotForLicensee:DataProtection:Keys");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                options.InstanceName = $"ChatBotForLicensee:Session:";
            });

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout =
                    TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:SessionTimeOut").Value));
                options.Cookie.Name = "Fanex.NCB.Session";
            });

            services.RegisterJWTAuthenticate(Configuration);

            services.AddNapServices(Configuration);
            services.AddTransient<ILogger, Logger>();
            services.AddOneBookUmForWeb(Configuration);

            services.RegisterServices();
            services.RegisterRepositories();

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            }).AddStackExchangeRedis(redisConfig, options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal(SignalRPrefix);
            });
        }

        private static void RegisterAuthenticationCookie(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
                    options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
                    options.ExpireTimeSpan =
                        TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
                    options.Cookie = new CookieBuilder
                    {
                        Name = "Fanex.NCB.Cookie",
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    options.Events.OnSigningOut = context =>
                    {
                        context.Response.Redirect(options.LoginPath);
                        return Task.CompletedTask;
                    };
                });
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<ErrorHandlingMiddleware>();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseHsts();
            }

            app.UseForwardedHeaders();

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == NotFoundStatus)
                {
                    context.Request.Path = "/Error/PageNotFound";
                    await next();
                }
            });

            NapClientManager
                .UseConfig(app.ApplicationServices.GetService<INapSettings>())
                .UseSession(app.ApplicationServices.GetService<INapSession>());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatbotHub>("/chatbot/hubs",
                    options => { options.Transports = HttpTransportType.WebSockets; });

                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Auth}/{action=Index}/{id?}");
            });
        }
    }
}