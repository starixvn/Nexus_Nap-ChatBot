namespace Fanex.NCB.Main
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Sentry;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.AppConfig;
    using Fanex.NCB.Main.Hubs;
    using Fanex.NCB.NAPResolver;
    using Fanex.NCB.Shared.Middlewares;
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
    using ONEbook.UM.AspNetCore;
    using ONEbook.UM.Configurations;
    using Sentry;
    using StackExchange.Redis;

    [ExcludeFromCodeCoverage(Justification = "Startup code inject services")]
    public class Startup
    {
        private const int NotFoundStatus = 404;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFanexSql(Configuration).AddFanexMySql(Configuration);

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

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

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
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:SessionTimeOut").Value));
                options.Cookie.Name = "Fanex.NCB.Session";
            });

            services.RegisterJWTAuthenticate(Configuration);

            services.AddNapServices(Configuration);
            services.AddTransient<ILogger, Logger>();
            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));

            services.RegisterServices();
            services.RegisterRepositories();

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            }).AddStackExchangeRedis(redisConfig, options =>
            {
                options.Configuration.ChannelPrefix = "ChatBotForLicensee:SignalR";
            });
        }

        private static void RegisterAuthenticationCookie(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                       .AddCookie(options =>
                       {
                           options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
                           options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
                           options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
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
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseForwardedHeaders();
                app.UseHsts();
            }

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
                endpoints.MapHub<ChatbotHub>("/chatbot/hubs", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}/{id?}");
            });
        }
    }
}