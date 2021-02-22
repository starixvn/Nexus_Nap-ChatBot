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
        private const short DisableConcurrentExecutionTimeoutInSeconds = 30;
        private const int MaxRandom = 1000;
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
        [DisableConcurrentExecution(DisableConcurrentExecutionTimeoutInSeconds)]
        public async Task PullMessageFromSpecialDelete()
        {
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            Random random = new Random();
            timeStamp = timeStamp + random.Next(0, MaxRandom).ToString();

            logger.WriteInfo($"{timeStamp} - Start PullMessageFromSpecialDelete job");
            if (await IsNotUM())
            {
                try
                {
                    logger.WriteInfo($"{timeStamp} - Inprogress PullMessageFromSpecialDelete job - after Check UM");
                    await Task.WhenAll(
                        specialDeleteTicketService.VoidingTicketToLicensee(),
                        specialDeleteTicketService.VoidingAbnormalBetsToInternalDepartment()
                    );

                    logger.WriteInfo($"{timeStamp} - Inprogress PullMessageFromSpecialDelete job - after pull msg");

                    await sendNotificationService.ScanAndSendNotification();
                    logger.WriteInfo($"{timeStamp} - Inprogress PullMessageFromSpecialDelete job - after send notification");
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
        [DisableConcurrentExecution(DisableConcurrentExecutionTimeoutInSeconds)]
        public async Task PullMessageFromSiteMessage()
        {
            if (await IsNotUM())
            {
                try
                {
                    await siteMessageService.PullMessageFromSiteMessageThenInsertNotification();
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