namespace Fanex.NCB.AutoJobs.Core.Tests.Services.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Models.Common;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;
    using Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket;
    using Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.AbnormalBets;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class SpecialDeleteServiceTestable : SpecialDeleteService
    {
        public SpecialDeleteServiceTestable(
            IBaseService baseService,
            IChatbotService chatbotService,
            IConfiguration configuration,
            ICommonService commonService,
            IRefNoTicketService refNoTicketService) : base(baseService, chatbotService, configuration, commonService, refNoTicketService)
        {
        }

        public async Task Public_UpdateMessageHistoryVoidingForLicenseeAsync(IEnumerable<MessageDetail> msgDetails)
        {
            await base.UpdateMessageHistoryVoidingForLicenseeAsync(msgDetails);
        }

        public async Task<(CommandResult, IEnumerable<NotificationMessage>)> Public_ImportMessageToChatbotDBAsync(
            IEnumerable<SpecialDeleteIssueType> messages,
            IEnumerable<MessageDetail> messageDetails)
        {
            return await base.ImportMessageToChatbotDBAsync(messages, messageDetails);
        }

        public async Task<(IEnumerable<Message>, IEnumerable<MessageDetail>)> Public_GetMessageFromVoidingForLicenseeAsync(SystemSetting systemSetting)
        {
            return await base.GetMessageFromVoidingForLicenseeAsync(systemSetting);
        }

        public async Task Public_ImportAbnormalBetNotificationToChatbotDBAsync(
            IEnumerable<AbnormalMessageDetail> messages,
            IEnumerable<AbnormalMessage> msgHistories,
            IEnumerable<DuplicateTicketDetail> duplicateTicketDetails,
            IEnumerable<AbnormalAccountInfo> abnormalAccountInfos,
            int maxMsgTargetId)
        {
            await base.ImportAbnormalBetNotificationToChatbotDBAsync(messages, msgHistories, duplicateTicketDetails, abnormalAccountInfos, maxMsgTargetId);
        }

        public async Task Public_InsertToMessageChatbotAsync(string messages, int jobIdentity)
        {
            await base.InsertToMessageChatbotAsync(messages, jobIdentity);
        }

        public async Task<(IEnumerable<AbnormalMessageContent>, IEnumerable<MessageMsgDetail>, int)> Public_GetAbnormalMessageDetail(
            IEnumerable<AbnormalMessage> messageIdLst,
            int batchSize,
            int lastMsgTargetId
        )
        {
            return await base.GetAbnormalMessageDetail(messageIdLst, batchSize, lastMsgTargetId);
        }

        public async Task<(IEnumerable<AbnormalMessageDetail>, IEnumerable<AbnormalMessage>, IEnumerable<AbnormalAccountInfo>, IEnumerable<DuplicateTicketDetail>)>
        Public_GetVoidingHistoryInternalCSDInfoAsync(
            IEnumerable<AbnormalMessageContent> messageInfo,
            IEnumerable<MessageMsgDetail> messageDetail
        )
        {
            return await base.GetVoidingHistoryInternalCSDInfoAsync(messageInfo, messageDetail);
        }

        public async Task Public_GetMessageAndImportToChatbotAsync(ServiceSetting setting, IEnumerable<AbnormalMessage> messageIdLst)
        {
            await base.GetMessageAndImportToChatbotAsync(setting, messageIdLst);
        }

        public async Task Public_GetTransInfoAsync(IEnumerable<DuplicateTicketDetail> dupTicketDetails)
        {
            await base.GetTransInfoAsync(dupTicketDetails);
        }

        public static void Public_FormatTelegramData(IEnumerable<AbnormalMessageDetail> historyDetail)
        {
            FormatTelegramData(historyDetail);
        }

        public async Task Public_FetchVoidingHistoryInfo(ICriteria criteria)
        {
            await base.FetchVoidingHistoryInfo(criteria);
        }
    }
}
