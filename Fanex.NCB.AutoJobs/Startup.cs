namespace Fanex.NCB.AutoJobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Services;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Hangfire;
    using Hangfire.MySql.Core;
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

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            };
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
            {
                options.LoginPath = Configuration.GetSection("AuthSettings:LoginPath").Value;
                options.LogoutPath = Configuration.GetSection("AuthSettings:LogoutPath").Value;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration.GetSection("AuthSettings:CookieTimeOut").Value));
                options.Cookie = new CookieBuilder
                {
                    Name = "Fanex.NCB.AutoJobs.Cookie",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
                options.Events.OnSigningOut = context =>
                {
                    context.Response.Redirect(options.LoginPath);
                    return Task.CompletedTask;
                };
            });

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
            services.Configure<List<HangfireJobSetting>>(Configuration.GetSection("HangfireJobSettings"));
            services.Configure<BotSettings>(Configuration.GetSection("ChatBotConfigs"));
            services.Configure<NotifyChatGroupOption>(Configuration.GetSection("NotifyChatGroupOptions"));
            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));
            services.AddTransient<ILogger, Logger>();
            RegisterService(services);
            RegisterRepository(services);

            RegisterHangfire(services, connections.FirstOrDefault(x => x.Key.Equals("Hangfire")).Value.ConnectionString);

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
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            }).UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = Configuration.GetValue<int>("WorkerCount"),
                ServerName = Environment.MachineName
            });

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Auth}/{action=Login}/{id?}");
            });
        }

        //Extensions
        private static void RegisterService(IServiceCollection services)
        {
            #region ONEbookUM

            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();
            services.AddSession();

            #endregion ONEbookUM

            services.AddSingleton<IBaseService, BaseService>();
            services.AddScoped<IChatbotService, ChatbotService>();
            services.AddScoped<ISendNotificationService, SendNotificationService>();
            services.AddScoped<ISyncDataService, SyncDataService>();
            services.AddScoped<ISpecialDeleteTicketService, SpecialDeleteTicketService>();
            services.AddScoped<IJobService, JobService>();
            services.AddTransient<IRecurringJobManager, RecurringJobManager>();
        }

        private static void RegisterRepository(IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<ISyncDataRepository, SyncDataRepository>();
        }

        private static void RegisterHangfire(IServiceCollection services, string connectionString)
        {
            var connection = Cryptography.DecryptConnectionString(connectionString);
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(connection)));
        }
    }
}