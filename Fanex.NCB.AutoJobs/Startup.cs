namespace Fanex.NCB.AutoJobs
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Services;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Middlewares;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Hangfire;
    using Hangfire.Redis;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using ONEbook.UM.AspNetCore;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using Sentry;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Startup
    {
        private readonly ConnectionMultiplexer Redis;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Redis = ConnectionMultiplexer.Connect(Configuration.GetSection("RedisConfig:ConnectionString").Value);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connections = Configuration.GetSection("ConnectionStrings")
                                    .GetChildren()
                                    .AsEnumerable()
                                    .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                                    .ToDictionary(x => x.GetValue<string>("Name"),
                                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));
            string spSettings = Configuration["Environment"] == null ?
                                "~/App_Data" :
                                $"~/App_Data/{Configuration["Environment"]}";

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithMySql()
               .WithSqlServer()
               .UseDefaultDbSettingProvider(spSettings)
               .Run();

            LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
            });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            //JsonConvert.DefaultSettings = () =>
            //{
            //    return new JsonSerializerSettings
            //    {
            //        DateTimeZoneHandling = DateTimeZoneHandling.Utc
            //    };
            //};
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //{
            //    options.LoginPath = Configuration.GetSection("AuthSettings:LoginPath").Value;
            //    options.LogoutPath = Configuration.GetSection("AuthSettings:LogoutPath").Value;
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:CookieTimeOut").Value));
            //    options.Cookie = new CookieBuilder
            //    {
            //        Name = "Fanex.NCB.AutoJobs.Cookie",
            //        SecurePolicy = CookieSecurePolicy.SameAsRequest
            //    };
            //    options.Events.OnSigningOut = context =>
            //    {
            //        context.Response.Redirect(options.LoginPath);
            //        return Task.CompletedTask;
            //    };
            //});

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:SessionTimeOut").Value));
                options.Cookie.Name = "Fanex.NCB.AutoJobs.Session";
            });

            services.AddSingleton(Configuration);
            services.Configure<IdentifyModel>(Configuration.GetSection("HangfireAuthenticate"));
            services.Configure<MonitorChatbot>(Configuration.GetSection("MonitorChatbot"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<SignalRSettings>(Configuration.GetSection("SignalRSettings"));
            services.Configure<List<HangfireJobSetting>>(Configuration.GetSection("HangfireJobSettings"));
            services.Configure<BotSettings>(Configuration.GetSection("ChatBotConfigs"));
            services.Configure<NotifyChatGroupOption>(Configuration.GetSection("NotifyChatGroupOptions"));
            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));
            services.AddTransient<ILogger, Logger>();
            services.AddTransient<ILogService, LogServiceExtend>();

            RegisterService(services, Configuration);
            RegisterRepository(services);

            services.AddSignalR();

            RegisterHangfire(services, Redis, Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting();
            //app.UseAuthentication();

            app.UseHangfireDashboard().UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = Configuration.GetValue<int>("HangfireConfigurations:WorkerCount"),
                ServerName = Environment.MachineName,
                Queues = new[] { QueuPriority.Critical, QueuPriority.Default }
            });

            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}/{id?}");
            });
        }

        //Extensions
        private static void RegisterService(IServiceCollection services, IConfiguration Configuration)
        {
            #region ONEbookUM

            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();

            #endregion ONEbookUM

            services.AddHttpClient();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IChatbotService, ChatbotService>();
            services.AddScoped<ISendNotificationService, SendNotificationService>();
            services.AddScoped<ISyncDataService, SyncDataService>();
            services.AddScoped<ISpecialDeleteService, SpecialDeleteService>();
            services.AddScoped<ISiteMessageService, SiteMessageService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IPushSignalService, PushSignalService>();
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddTransient<IRecurringJobManager, RecurringJobManager>();
            services.AddScoped<SignalRService>();
        }

        private static void RegisterRepository(IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<ISyncDataRepository, SyncDataRepository>();
        }

        private static void RegisterHangfire(IServiceCollection services, ConnectionMultiplexer redis, IConfiguration configuration)
        {
            services.AddHangfire(x =>
            {
                x.UseRedisStorage(redis, new RedisStorageOptions
                {
                    Prefix = configuration.GetSection("HangfireConfigurations:Prefix")?.Value,
                    SucceededListSize = int.Parse(configuration.GetSection("HangfireConfigurations:SucceededListSize")?.Value),
                    DeletedListSize = int.Parse(configuration.GetSection("HangfireConfigurations:DeletedListSize")?.Value),
                    Db = int.Parse(configuration.GetSection("HangfireConfigurations:Db")?.Value)
                });
            });
        }
    }
}