namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Hangfire;
    using Microsoft.Extensions.Options;

    public class JobService : IJobService
    {
        private readonly ISendNotificationService sendNotificationService;
        private readonly ISyncDataService syncDataService;
        private readonly IChatbotService chatbotService;
        private readonly ILogger logger;
        private readonly IOptionsSnapshot<MonitorChatbot> monitorOptions;

        public JobService(ISendNotificationService sendNotificationService,
            ISyncDataService syncDataService,
            IChatbotService chatbotService,
            ILogger logger,
            IOptionsSnapshot<MonitorChatbot> monitorOptions)
        {
            this.sendNotificationService = sendNotificationService;
            this.syncDataService = syncDataService;
            this.chatbotService = chatbotService;
            this.monitorOptions = monitorOptions;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task AutoScanAndSendNotifictaion()
        {
            try
            {
                await sendNotificationService.ScanAndSendNotification();
            }
            catch (Exception ex)
            {
                await Logging(ex);
                throw;
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromNapUserControl()
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

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromOIC()
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

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task MonitorChatbot()
        {
            try
            {
                var settings = monitorOptions.Value;
                if (settings.IsMonitoring)
                {
                    var notificationModel = new List<NotificationMessage>
                    {
                        new NotificationMessage
                        {
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
            }
            catch (Exception ex)
            {
                await Logging(ex);
                throw;
            }
        }

        private async Task Logging(Exception exception)
        {
            string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    exception.Message,
                    exception.Source,
                    string.Empty,
                    exception.TargetSite,
                    exception.StackTrace);
            await logger.ErrorAsync(message);
        }
    }
}