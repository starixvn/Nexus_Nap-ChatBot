namespace Fanex.NCB.Services.Tests.TestableClasses
{
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

        public async Task<IEnumerable<VoidingHistoryUsername>> Public_GetAbnormalAccountUpdatedUsernameAsync(IEnumerable<AbnormalMessageHistory> messages)
        {
            return await base.GetAbnormalAccountUpdatedUsernameAsync(messages);
        }

        public async Task<List<MessageHistory>> Public_GetMessageHistoryAsync()
        {
            return await base.GetMessageHistoryAsync();
        }

        public async Task<SystemSetting> Public_GetAbnormalBetSystemSettingAsync()
        {
            return await base.GetAbnormalBetSystemSettingAsync();
        }

        public async Task<IEnumerable<Message>> Public_GetAbnormalBetsMessageAsync(
            SystemSetting setting,
            List<MessageHistory> listMsgs)
        {
            return await base.GetAbnormalBetsMessageAsync(setting, listMsgs);
        }

        public async Task<(IEnumerable<AbnormalMessage>, IEnumerable<AbnormalMessageHistory>)> Public_GetVoidingHistoryAsync(List<MessageHistory> messages)
        {
            return await base.GetVoidingHistoryAsync(messages);
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