namespace Fanex.NCB.AutoJobs.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentInternalCSD;
    using Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentLicensee;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;
    using Fanex.NCB.AutoJobs.Core.Models.ResultAmendment;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Newtonsoft.Json;

    public class ResultAmendmentService : IResultAmendmentService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly ICommonService commonService;

        public ResultAmendmentService
        (
            IBaseService baseService,
            IChatbotService chatbotService,
            ICommonService commonService
        )
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.commonService = commonService;
        }

        #region ResultAmendment Internal CSD

        public async Task ResultAmendmentInternalCSDAsync()
        {
            var systemSetting = await commonService.GetServiceSetting(Constants.Constants.ServiceName_ResultAmendmentInternalCSD);

            if (systemSetting != null)
            {
                var criteria = new GetMessageResultAmendmentCsdCriteria
                {
                    LastModifiedTime = systemSetting.LastModifiedTime
                };
                var messages = await baseService.GetDataAsync<MessageAmendmentCsd>(criteria);
                await ImportResultAmendmentInternalCSDAsync(messages, criteria.NewLastModifiedTime);
            }
        }

        protected internal async Task ImportResultAmendmentInternalCSDAsync(IEnumerable<MessageAmendmentCsd> messages, DateTime? newLastModifiedTime)
        {
            if (messages.Any())
            {
                var criteria = new ImportResultAmendmentCSDCommand
                {
                    LastModifiedTime = newLastModifiedTime,
                    Message = JsonConvert.SerializeObject(messages)
                };
                var (recipients, notifications) = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
                var notificationMessages = chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);
                var command = new CommandResult
                {
                    Code = criteria.ErrCode,
                    Message = criteria.ErrMsg
                };

                await commonService.OnAfterImportDataAsync(command, notificationMessages);
            }
        }

        #endregion ResultAmendment Internal CSD

        #region ResultAmendment Licensee

        public async Task ResultAmendmentLicenseeAsync()
        {
            var systemSetting = await commonService.GetServiceSetting(Constants.Constants.ServiceName_ResultAmendmentLicensee);

            if (systemSetting != null)
            {
                var criteria = new GetMessageResultAmendmentLicenseeCriteria
                {
                    BatchSize = systemSetting.BatchSize,
                    LastMsgTargetId = systemSetting.LastMsgTargetId,
                    SiteAsJSON = await commonService.GetJsonDataFromDbACCAsync("Site"),
                    DepCustSAsJSON = await commonService.GetJsonDataFromDbACCAsync("Dep_CustSuper")
                };
                var (messageHeaders, messageDetails, messageTargets) = await baseService.FetchMultipleAsync<MessageAmendmentHeader, MessageAmendmentDetail, MessageAmendmentTarget>(criteria);
                await ImportResultAmendmentInternalLicenseeAsync(messageHeaders, messageDetails, messageTargets);
            }
        }

        protected internal async Task ImportResultAmendmentInternalLicenseeAsync(
            IEnumerable<MessageAmendmentHeader> messageHeaders,
            IEnumerable<MessageAmendmentDetail> messageDetails,
            IEnumerable<MessageAmendmentTarget> messageTargets
        )
        {
            if (messageHeaders.Any())
            {
                var criteria = new ImportResultAmendmentLicenseeCommand
                {
                    MessageHeader = JsonConvert.SerializeObject(messageHeaders),
                    MessageDetail = JsonConvert.SerializeObject(messageDetails),
                    MessageTarget = JsonConvert.SerializeObject(messageTargets)
                };
                var (recipients, notifications) = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
                var notificationMessages = chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);
                var command = new CommandResult
                {
                    Code = criteria.ErrCode,
                    Message = criteria.ErrMsg
                };

                await commonService.OnAfterImportDataAsync(command, notificationMessages);
            }
        }

        #endregion ResultAmendment Licensee
    }
}