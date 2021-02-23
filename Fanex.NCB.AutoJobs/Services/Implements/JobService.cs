namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using ONEbook.UM.Services;

    public class JobService : IJobService
    {
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

        public async Task PullMessageFromSpecialDelete()
        {
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            Random random = new Random();
            timeStamp += random.Next(0, MaxRandom).ToString();

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
                }
                catch (Exception ex)
                {
                    await logger.WriteError(ex);
                    throw;
                }
            }
        }

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