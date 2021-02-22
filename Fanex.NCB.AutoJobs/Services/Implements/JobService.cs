namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;
    using ONEbook.UM.Services;
    using System;
    using System.Threading.Tasks;

    public class JobService : IJobService
    {
        private readonly ISyncDataService syncDataService;
        private readonly IChatbotService chatbotService;
        private readonly ISpecialDeleteService specialDeleteTicketService;
        private readonly ISendNotificationService sendNotificationService;
        private readonly IMaintenanceService maintenanceService;
        private readonly ISiteMessageService siteMessageService;
        private readonly ILogServiceExt logger;

        public JobService(ISyncDataService syncDataService,
            IChatbotService chatbotService,
            ISpecialDeleteService specialDeleteTicketService,
            ISendNotificationService sendNotificationService,
            IMaintenanceService maintenanceService,
            ISiteMessageService siteMessageService,
            ILogServiceExt logger)
        {
            this.syncDataService = syncDataService;
            this.chatbotService = chatbotService;
            this.specialDeleteTicketService = specialDeleteTicketService;
            this.sendNotificationService = sendNotificationService;
            this.maintenanceService = maintenanceService;
            this.siteMessageService = siteMessageService;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
        public async Task SyncDataFromNapUserControl()
        {
            if (await IsNotUM())
            {
                try
                {
                    await syncDataService.NapUserControlSyncDataAsync();
                }
                catch (Exception ex)
                {
                    await logger.WriteError(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
        public async Task SyncDataFromOIC()
        {
            if (await IsNotUM())
            {
                try
                {
                    await syncDataService.OicSyncDataAsync();
                }
                catch (Exception ex)
                {
                    await logger.WriteError(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Critical)]
        public async Task PullMessageFromSpecialDelete()
        {
            logger.WriteInfo("");
            if (await IsNotUM())
            {
                try
                {
                    await Task.WhenAll(
                        specialDeleteTicketService.VoidingTicketToLicensee(),
                        specialDeleteTicketService.VoidingAbnormalBetsToInternalDepartment()
                    );

                    await sendNotificationService.ScanAndSendNotification();
                }
                catch (Exception ex)
                {
                    await logger.WriteError(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Critical)]
        public async Task PullMessageFromSiteMessage()
        {
            if (await IsNotUM())
            {
                try
                {
                    await Task.WhenAll(
                        siteMessageService.PullMessageFromSiteMessageThenInsertNotification()
                    );

                    await sendNotificationService.ScanAndSendNotification();
                }
                catch (Exception ex)
                {
                    await logger.WriteError(ex);
                    throw;
                }
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Critical)]
        public async Task ScanAndSendNotification()
        {
            try
            {
                await sendNotificationService.ScanAndSendNotification();
            }
            catch (Exception ex)
            {
                await logger.WriteError(ex);
                throw;
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
        public async Task MonitorChatbot()
        {
            try
            {
                await chatbotService.MonitorChatbot();
            }
            catch (Exception ex)
            {
                await logger.WriteError(ex);
                throw;
            }
        }

        private async Task<bool> IsNotUM()
        {
            var maintanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
            return !maintanceTime.IsUnderMaintenanceTime;
        }
    }
}