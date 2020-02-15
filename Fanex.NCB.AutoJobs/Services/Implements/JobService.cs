namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;

    public class JobService : IJobService
    {
        private readonly ISyncDataService syncDataService;
        private readonly IChatbotService chatbotService;
        private readonly ISpecialDeleteService specialDeleteTicketService;
        private readonly ISendNotificationService sendNotificationService;
        private readonly ILogger logger;

        public JobService(ISyncDataService syncDataService,
            IChatbotService chatbotService,
            ISpecialDeleteService specialDeleteTicketService,
            ISendNotificationService sendNotificationService,
            ILogger logger)
        {
            this.syncDataService = syncDataService;
            this.chatbotService = chatbotService;
            this.specialDeleteTicketService = specialDeleteTicketService;
            this.sendNotificationService = sendNotificationService;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
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
        [Queue(QueuPriority.Default)]
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
        [Queue(QueuPriority.Critical)]
        public async Task EveryMinuteJobs()
        {
            try
            {
                await Task.WhenAll(
                    Task.Run(() => specialDeleteTicketService.VoidingTicketToLicensee()),
                    Task.Run(() => specialDeleteTicketService.VoidingAbnormalBetsToInternalDepartment())
                );
            }
            catch (Exception ex)
            {
                await Logging(ex);
                throw;
            }
            finally
            {
                await sendNotificationService.ScanAndSendNotification();
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
                await Logging(ex);
                throw;
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
    }
}