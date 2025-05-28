using Fanex.Data;
using ONEbook.UM.DependencyInjection;

namespace Fanex.NCB.AutoJobs
{
    using Caching;
    using Caching.Redis;
    using Core.Repository;
    using Core.Services;
    using Core.Services.Interfaces;
    using Data.MySql;
    using Data.Repository;
    using Data.SqlServer;
    using Fanex.NCB.AutoJobs.Core.Models.Configurations;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using RemotingRef;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Workers;

    [ExcludeFromCodeCoverage(Justification = "Code Starup inject needed services")]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            DbSettingProviderManager.StartNewSession()
                .WithSqlServer(Configuration)
                .WithMySql(Configuration)
                .UseDefaultDbSettingProvider("App_Data/SpNames.xml", true)
                .Run();

            services.AddControllers();
            AddConfiguration(services, Configuration);
            AddRedisCacheConfig(services);
            AddDependencyInjection(services, Configuration);
            AddWorker(services);
        }

        private void AddRedisCacheConfig(IServiceCollection services)
        {
            var redis = ConnectionMultiplexer.Connect(
                Configuration.GetSection("Fanex.Caching.Redis:ConnectionString").Value);
            services.AddDataProtection()
                .SetApplicationName(Configuration.GetSection("Fanex.Caching.Redis:ApplicationName").Value)
                .PersistKeysToStackExchangeRedis(redis, "Fanex.NCB.AutoJobs.AutoJobs-Keys");

            services.AddRedisCache(Configuration.GetSection("Fanex.Caching.Redis"));

            const short WorkerThreads = 300;
            const short CompletionPortThreads = 300;
            _ = ThreadPool.SetMinThreads(WorkerThreads, CompletionPortThreads);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Auth}/{action=Index}/{id?}");
            });
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            RefDataTypeContainer
                .ConnectTo(new Uri(configuration.GetSection("RemotingRefService:Fanex.RemotingRef.Service.Url").Value))
                .FromApplication("Fanex.NCB.AutoJobs");

            services.Configure<IConfiguration>(configuration);
            services.AddSingleton<IRemotingRefService, RemotingRefService>();

            services.AddSingleton(configuration);
            services.Configure<MonitorChatbot>(configuration.GetSection("MonitorChatbot"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.Configure<SignalRSettings>(configuration.GetSection("SignalRSettings"));
            services.Configure<List<JobSetting>>(configuration.GetSection("JobSettings"));
            services.Configure<BotSettings>(configuration.GetSection("ChatBotConfigs"));
            services.Configure<NotifyChatGroupOption>(configuration.GetSection("NotifyChatGroupOptions"));
            services.Configure<MonitorTelegramBot>(configuration.GetSection("MonitorTelegramBot"));
        }

        private static void AddDependencyInjection(IServiceCollection services, IConfiguration configuration)
        {
            #region ONEbookUM

            services.AddOneBookUmForService(configuration);

            #endregion ONEbookUM

            services.AddSingleton<ILogService, LogServiceExtend>();

            services.AddHttpClient();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IChatbotService, ChatbotService>();
            services.AddSingleton<ISendNotificationService, SendNotificationService>();
            services.AddSingleton<ISyncDataService, SyncDataService>();
            services.AddSingleton<ISpecialDeleteService, SpecialDeleteService>();
            services.AddSingleton<ISiteMessageService, SiteMessageService>();
            services.AddSingleton<IPushSignalRService, PushSignalRService>();
            services.AddSingleton<IRefNoTicketService, RefNoTicketService>();

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<ISyncDataRepository, SyncDataRepository>();

            services.AddSingleton<ICommonService, CommonService>();
            services.AddSingleton<IProblemAccountService, ProblemAccountService>();
            services.AddSingleton<IMonitoredPlayerService, MonitoredPlayerService>();
            services.AddSingleton<ISiteDbService, SiteDbService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ISettlementInternalLicenseeService, SettlementInternalLicenseeService>();
            services.AddSingleton<IResultAmendmentService, ResultAmendmentService>();
            services.AddSingleton<IMonitorMatchService, MonitorMatchService>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            //services.AddHostedService<ScanAndSendNotificationWorker>();
            //services.AddHostedService<SyncDataFromNapUserControlWorker>();
            //services.AddHostedService<SyncDataFromOICWorker>();
            //services.AddHostedService<PullVoidingMessageLicenseeWorker>();
            //services.AddHostedService<PullVoidingMessageInternalDepartmentWorker>();
            //services.AddHostedService<PullMessageFromSiteMessage>();
            //services.AddHostedService<MonitorChatbotWorker>();
            //services.AddHostedService<MonitorTelegramBotWorker>();
            //services.AddHostedService<DeleteOldConversationMessageLogWorker>();
            //services.AddHostedService<SendMessageProblemAccountWorker>();
            //services.AddHostedService<SendMessageMonitoredPlayerWorker>();
            //services.AddHostedService<SettlementInternalCSDWorker>();
            //services.AddHostedService<SettlementLicenseeWorker>();
            //services.AddHostedService<ResultAmendmentInternalCSDWorkder>();
            //services.AddHostedService<ResultAmendmentLicenseeWorkder>();
            services.AddHostedService<MonitorMatchWorker>();
        }
    }
}