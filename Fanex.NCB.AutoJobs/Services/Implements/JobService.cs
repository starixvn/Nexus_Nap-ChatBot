namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Hangfire;
    using Microsoft.Extensions.Options;

    public class JobService : IJobService
    {
        private readonly ISyncDataService syncDataService;
        private readonly IChatbotService chatbotService;
        private readonly ISpecialDeleteTicketService specialDeleteTicketService;
        private readonly ILogger logger;
        private readonly IOptionsMonitor<List<HangfireJobSetting>> jobSettings;
        private readonly IOptionsSnapshot<MonitorChatbot> monitorOptions;

        public JobService(ISyncDataService syncDataService,
            IChatbotService chatbotService,
            ISpecialDeleteTicketService specialDeleteTicketService,
            ILogger logger,
            IOptionsMonitor<List<HangfireJobSetting>> jobSettings,
            IOptionsSnapshot<MonitorChatbot> monitorOptions)
        {
            this.syncDataService = syncDataService;
            this.chatbotService = chatbotService;
            this.specialDeleteTicketService = specialDeleteTicketService;
            this.monitorOptions = monitorOptions;
            this.logger = logger;
            this.jobSettings = jobSettings;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromNapUserControl()
        {
            var configs = GetJobConfig(HangfireJob.SyncNapUserControlData);
            if (configs != null && configs.IsEnabled)
            {
                try
                {
                    await syncDataService.NapUserControlSyncDataAsync();
                }
                catch (Exception ex)
                {
                    await Logging(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromOIC()
        {
            var configs = GetJobConfig(HangfireJob.SyncOICData);
            if (configs != null && configs.IsEnabled)
            {
                try
                {
                    await syncDataService.OicSyncDataAsync();
                }
                catch (Exception ex)
                {
                    await Logging(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task PullTicketCreateNotificationAndScanNotification()
        {
            var configs = GetJobConfig(HangfireJob.PullTicketFromSpecialDeleteAndScanNoti);
            if (configs != null && configs.IsEnabled)
            {
                try
                {
                    await specialDeleteTicketService.PullTicketAndCreateNotification();
                }
                catch (Exception ex)
                {
                    await chatbotService.NotifyToGroup(ex.Message);
                    await Logging(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task MonitorChatbot()
        {
            var configs = GetJobConfig(HangfireJob.MonitorSkypebot);
            if (configs != null && configs.IsEnabled)
            {
                try
                {
                    var settings = monitorOptions.Value;
                    var notificationModel = new List<NotificationMessage>
                    {
                        new NotificationMessage {
                        NotifictaionId = -1,
                        Message = settings.Message,
                        Recipients = new List<ConversationInfo>
                            {
                                new ConversationInfo
                                {
                                    ConversationId = settings.ConversationId,
                                    ChannelId = settings.ChannelId,
                                    ServiceUrl = settings.ServiceUrl,
                                    BotConversationId = settings.BotConversationId,
                                    BotName = settings.BotName
                                }
                            }
                        }
                    };
                    await chatbotService.SendMessageToMultitpleAsync(notificationModel);
                }
                catch (Exception ex)
                {
                    await Logging(ex);
                    throw;
                }
            }
        }

        private async Task Logging(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                       .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                       .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                       .ToString();
            await logger.ErrorAsync(logMessage);
        }

        private HangfireJobSetting GetJobConfig(string jobName)
        {
            return jobSettings.CurrentValue.FirstOrDefault(job => job.JobName.Equals(jobName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}