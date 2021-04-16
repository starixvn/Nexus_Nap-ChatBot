namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class JobService : IJobService
    {
        private readonly ISyncDataService syncDataService;
        private readonly IChatbotService chatbotService;
        private readonly ISpecialDeleteService specialDeleteTicketService;
        private readonly ISendNotificationService sendNotificationService;
        private readonly IMaintenanceService maintenanceService;
        private readonly ISiteMessageService siteMessageService;
        private readonly IOptionsSnapshot<List<HangfireJobSetting>> jobSettings;
        private readonly ILogService logger;

        public JobService(ISyncDataService syncDataService,
            IChatbotService chatbotService,
            ISpecialDeleteService specialDeleteTicketService,
            ISendNotificationService sendNotificationService,
            IMaintenanceService maintenanceService,
            ISiteMessageService siteMessageService,
            IOptionsSnapshot<List<HangfireJobSetting>> jobSettings,
            ILogService logger)
        {
            this.syncDataService = syncDataService;
            this.chatbotService = chatbotService;
            this.specialDeleteTicketService = specialDeleteTicketService;
            this.sendNotificationService = sendNotificationService;
            this.maintenanceService = maintenanceService;
            this.siteMessageService = siteMessageService;
            this.jobSettings = jobSettings;
            this.logger = logger;
        }

        public async Task ScanAndSendNotification()
        {
            try
            {
                if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.SendNotification))
                {
                    //await sendNotificationService.ScanAndSendNotification();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                }
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        public async Task SyncDataFromNapUserControl()
        {
            try
            {
                if (!await IsUM() && HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.SyncNapUserControlData))
                {
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                } 
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        public async Task SyncDataFromOIC()
        {
            try
            {
                if (!await IsUM() && HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.SyncOICData))
                {
                    //await syncDataService.OicSyncDataAsync();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                }
                        
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        public async Task PullVoidingMessageLicensee()
        {
            try
            {
                if (!await IsUM() && HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJob.PullVoidingMessageLicensee))
                {
                    //await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                }
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        public async Task PullVoidingMessageInternalDepartment()
        {
            try
            {
                if (!await IsUM() && HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJob.PullVoidingMessageInternalDepartment))
                {
                    //await specialDeleteTicketService.PullVoidingMessageInternalDepartment();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                }
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        public async Task PullMessageFromSiteMessage()
        {
            try
            {
                if (!await IsUM() && HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJob.PullMessageFromSiteMessage))
                {
                    //await siteMessageService.PullMessageFromSiteMessage();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                }
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        public async Task MonitorChatbot()
        {
            try
            {
                if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.MonitorSkypebot))
                {
                    //await chatbotService.MonitorChatbot();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                    await specialDeleteTicketService.PullVoidingMessageLicensee();
                }     
            }
            catch (Exception ex)
            {
                await logger.WriteExceptionSentryErrorAsync(ex);
            }
        }

        private async Task<bool> IsUM()
        {
            var maintanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
            return maintanceTime.IsUnderMaintenanceTime;
        }
    }
}