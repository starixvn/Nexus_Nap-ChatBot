namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SiteMessage;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class SpecialDeleteServiceTestable : SpecialDeleteService
    {
        public SpecialDeleteServiceTestable(
            IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            ILogService logService,
            IConfiguration configuration) : base(baseService, chatbotService, pushSignalService, logService, configuration)
        {
        }

        public async Task Public_UpdateMessageHistoryVoidingForLicenseeAsync(CommandResult commandResult, IEnumerable<MessageDetail> msgDetails)
        {
            await base.UpdateMessageHistoryVoidingForLicenseeAsync(commandResult, msgDetails);
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

        public async Task<IEnumerable<VoidingHistoryUsername>> Public_GetAbnormalAccountUpdatedUsernameAsync(IEnumerable<AbnormalMessageHistory> messages)
        {
            return await base.GetAbnormalAccountUpdatedUsernameAsync(messages);
        }

        public async Task<IEnumerable<Message>> Public_GetAbnormalBetsMessageAsync(
            SystemSetting setting,
            IEnumerable<MessageHistory> listMsgs)
        {
            return await base.GetAbnormalBetsMessageAsync(setting, listMsgs);
        }

        public async Task<(IEnumerable<AbnormalMessage>, IEnumerable<AbnormalMessageHistory>)> Public_GetVoidingHistoryAsync(List<MessageHistory> messages)
        {
            return await base.GetVoidingHistoryAsync(messages);
        }

        public async Task Public_ImportAbnormalBetNotificationToChatbotDBAsync(
            IEnumerable<AbnormalMessage> messages,
            IEnumerable<MessageHistory> msgHistories,
            int maxMessageId)
        {
            await base.ImportAbnormalBetNotificationToChatbotDBAsync(messages, msgHistories, maxMessageId);
        }

        public async Task Public_InsertToMessageChatbotAsync(IEnumerable<MessageHistory> messages, int jobIdentity)
        {
            await base.InsertToMessageChatbotAsync(messages, jobIdentity);
        }

        public async Task Public_SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
        {
            await base.SendMessageByChannelAsync(channelId, notifications);
        }
    }
}