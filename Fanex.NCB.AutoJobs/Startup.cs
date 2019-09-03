namespace Fanex.NCB.AutoJobs
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
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
    using Hangfire.Dashboard;
    using Hangfire.MySql.Core;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
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

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithMySql()
               .WithSqlServer()
               .UseDefaultDbSettingProvider("~/App_Data")
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

            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            };

            services.AddSingleton(Configuration);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<BotSettings>(Configuration.GetSection("ChatBotConfigs"));
            services.Configure<CronJob>(Configuration.GetSection("CronJob"));
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

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>()
            }).UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = Configuration.GetValue<int>("WorkerCount")
            });

            app.UseStaticFiles();
            app.UseMvc();
        }

        //Extensions
        private void RegisterService(IServiceCollection services)
        {
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IChatbotService, ChatbotService>();
            services.AddSingleton<ISendNotificationService, SendNotificationService>();
            services.AddSingleton<ISyncDataService, SyncDataService>();
            services.AddSingleton<IJobService, JobService>();
        }

        private void RegisterRepository(IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<ISyncDataRepository, SyncDataRepository>();
        }

        private void RegisterHangfire(IServiceCollection services, string connectionString)
        {
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(connectionString)));
        }
    }
}