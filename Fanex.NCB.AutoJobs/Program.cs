namespace Fanex.NCB.AutoJobs
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.AutoJobs.Workers;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NLog.Web;
    using ONEbook.UM.AspNetCore;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using Sentry;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            Host.CreateDefaultBuilder(args)
                .UseNLog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    AddConfiguration(services, configuration);
                    AddDependencyInjection(services, configuration);
                    AddWorker(services);
                }).Build().Run();
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var connections = configuration.GetSection("ConnectionStrings")
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
                       .UseDefaultDbSettingProvider("~/App_Data/SpNames.xml")
                       .Run();

            LogManager
            .SetDefaultLogCategory(configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
            });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            services.AddSingleton(configuration);
            services.Configure<MonitorChatbot>(configuration.GetSection("MonitorChatbot"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.Configure<SignalRSettings>(configuration.GetSection("SignalRSettings"));
            services.Configure<List<JobSetting>>(configuration.GetSection("JobSettings"));
            services.Configure<BotSettings>(configuration.GetSection("ChatBotConfigs"));
            services.Configure<NotifyChatGroupOption>(configuration.GetSection("NotifyChatGroupOptions"));
        }

        private static void AddDependencyInjection(IServiceCollection services, IConfiguration configuration)
        {
            #region ONEbookUM

            services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();

            #endregion ONEbookUM

            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ILogService, LogServiceExtend>();

            services.AddHttpClient();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IChatbotService, ChatbotService>();
            services.AddSingleton<ISendNotificationService, SendNotificationService>();
            services.AddSingleton<ISyncDataService, SyncDataService>();
            services.AddSingleton<ISpecialDeleteService, SpecialDeleteService>();
            services.AddSingleton<ISiteMessageService, SiteMessageService>();
            services.AddSingleton<IPushSignalService, PushSignalService>();

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<ISyncDataRepository, SyncDataRepository>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<ScanAndSendNotificationWorker>();
            services.AddHostedService<SyncDataFromNapUserControlWorker>();
            services.AddHostedService<SyncDataFromOICWorker>();
            services.AddHostedService<PullVoidingMessageLicenseeWorker>();
            services.AddHostedService<PullVoidingMessageInternalDepartmentWorker>();
            services.AddHostedService<PullMessageFromSiteMessage>();
            services.AddHostedService<MonitorChatbotWorker>();
        }
    }
}