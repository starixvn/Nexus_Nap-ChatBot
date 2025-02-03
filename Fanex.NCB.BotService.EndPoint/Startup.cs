using Fanex.Logging.Sentry.RavenClient;
using SharpRaven;

namespace Fanex.NCB.BotService.EndPoint;

using Caching.Redis;
using Core;
using Data;
using Logging.Extensions;
using Logging.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Middlewares;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

[ExcludeFromCodeCoverage]
public class Startup
{
    private readonly ILogger logger;
    private readonly ConnectionMultiplexer Redis;

    public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        Configuration = configuration;
        Environment = env;
        Redis = ConnectionMultiplexer.Connect(Configuration.GetSection("Fanex.Caching.Redis:ConnectionString").Value);
        this.logger = logger;
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    [Obsolete]
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddFanexData(Configuration);

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        Logging.LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<Logging.ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value),
                    FrontEndDsn = new Dsn(Configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
                })
            });

        ExceptionHandler
            .UseConfig(new LoggingConfiguration());

        var appSettingsSection = Configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        IdentityModelEventSource.ShowPII = true;
        BotServiceDI.Init(services, Configuration, Environment, appSettings, logger);

        services.AddDataProtection()
            .SetApplicationName(Configuration.GetSection("Fanex.Caching.Redis:ApplicationName").Value)
            .PersistKeysToStackExchangeRedis(Redis, "Fanex.NCB.BotService.Endpoint-Keys");

        services.AddRedisCache(Configuration.GetSection("Fanex.Caching.Redis"));
        services.AddMvc().AddNewtonsoftJson();

        const short WorkerThreads = 300;
        const short CompletionPortThreads = 300;
        _ = ThreadPool.SetMinThreads(WorkerThreads, CompletionPortThreads);
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

        app.UseRouting();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseBotFramework();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });
    }
}