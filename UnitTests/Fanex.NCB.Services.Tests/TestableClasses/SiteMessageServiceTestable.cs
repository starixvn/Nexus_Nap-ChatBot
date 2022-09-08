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

    public class SiteMessageServiceTestable : SiteMessageService
    {
        public SiteMessageServiceTestable(
               IBaseService baseService,
               IChatbotService chatbotService,
               IPushSignalRService pushSignalService,
               ILogService logService,
               IConfiguration configuration)
            : base(baseService, chatbotService, pushSignalService, logService, configuration)
        { }

        public async Task<(CommandResult, IEnumerable<NotificationMessage>)> Public_ImportMessageToChatbotDBAsync(
            IEnumerable<SiteMessageModel> messages,
            IEnumerable<SiteMessageDetail> messageDetails)
        {
            return await base.ImportMessageToChatbotDBAsync(messages, messageDetails);
        }

        public async Task Public_OnAfterImportDataAsync(
            CommandResult commandResult,
            IEnumerable<NotificationMessage> notifications,
            IEnumerable<SiteMessageDetail> messageDetails)
        {
            await base.OnAfterImportDataAsync(commandResult, notifications, messageDetails);
        }

        public async Task Public_InsertMessageHistoryAsync(IEnumerable<SiteMessageDetail> messageDetails)
        {
            await base.InsertMessageHistoryAsync(messageDetails);
        }

        public async Task<IEnumerable<SiteMessageMatchInfoModel>> Public_GetMatchInfoAsync(string matchIds)
        {
            return await base.GetMatchInfoAsync(matchIds);
        }

        public async Task<(IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>)> Public_GetDataFromSiteMessageAsync(SystemSetting systemSetting)
        {
            return await base.GetDataFromSiteMessageAsync(systemSetting);
        }

        public async Task Public_SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
        {
            await base.SendMessageByChannelAsync(channelId, notifications);
        }
    }
}