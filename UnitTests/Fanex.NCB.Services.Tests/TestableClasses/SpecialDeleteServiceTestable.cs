namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class SpecialDeleteServiceTestable : SpecialDeleteService
    {
        public SpecialDeleteServiceTestable(
            IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalRService pushSignalService,
            ILogService logService,
            ISiteDbService siteDbService,
            IConfiguration configuration) : base(baseService, chatbotService, pushSignalService, logService, configuration, siteDbService)
        {
        }

        public async Task Public_UpdateMessageHistoryVoidingForLicenseeAsync(IEnumerable<MessageDetail> msgDetails)
        {
            await base.UpdateMessageHistoryVoidingForLicenseeAsync(msgDetails);
        }

        public async Task Public_OnAfterImportDataAsync(CommandResult commandResult, IEnumerable<NotificationMessage> notifications)
        {
            await base.OnAfterImportDataAsync(commandResult, notifications);
        }

        public async Task<(CommandResult, IEnumerable<NotificationMessage>)> Public_ImportMessageToChatbotDBAsync(
            IEnumerable<Message> messages,
            IEnumerable<MessageDetail> messageDetails)
        {
            return await base.ImportMessageToChatbotDBAsync(messages, messageDetails);
        }

        public async Task<(IEnumerable<Message>, IEnumerable<MessageDetail>)> Public_GetMessageFromVoidingForLicenseeAsync(SystemSetting systemSetting)
        {
            return await base.GetMessageFromVoidingForLicenseeAsync(systemSetting);
        }

        public async Task Public_GetMessageAndImportToChatbotAsync(IEnumerable<MessageHistory> messageHistories, SystemSetting setting)
        {
            await base.GetMessageAndImportToChatbotAsync(messageHistories, setting);
        }

        public async Task<IEnumerable<AbnormalMessage>> Public_GetVoidingHistoryAsync(
            IEnumerable<AbnormalTicketInfo> tickets,
            IEnumerable<AbnormalUsernames> usernames,
            IEnumerable<MessageHistory> messages)
        {
            return await base.GetVoidingHistoryAsync(tickets, usernames, messages);
        }

        public async Task Public_ImportAbnormalBetNotificationToChatbotDBAsync(
            IEnumerable<AbnormalMessage> messages,
            IEnumerable<MessageHistory> msgHistories,
            int maxMessageId)
        {
            await base.ImportAbnormalBetNotificationToChatbotDBAsync(messages, msgHistories, maxMessageId);
        }

        public async Task Public_InsertToMessageChatbotAsync(string messages, int jobIdentity)
        {
            await base.InsertToMessageChatbotAsync(messages, jobIdentity);
        }

        public async Task Public_SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
        {
            await base.SendMessageByChannelAsync(channelId, notifications);
        }
    }
}