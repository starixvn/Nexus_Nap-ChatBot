namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.Settlement;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class SettlementInternalLicenseeServiceTestable : SettlementInternalLicenseeService
    {
        public SettlementInternalLicenseeServiceTestable(
            IBaseService baseService,
            IPushSignalRService pushSignalService,
            IChatbotService chatbotService,
            ILogService logService,
            ICommonService commonService
        ) : base(baseService, pushSignalService, chatbotService, logService, commonService)
        {
        }

        public async Task Public_OnAfterImportDataAsync(CommandResult commandResult, IEnumerable<NotificationMessage> notifications)
        {
            await base.OnAfterImportDataAsync(commandResult, notifications);
        }

        public async Task Public_SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
        {
            await base.SendMessageByChannelAsync(channelId, notifications);
        }

        public async Task Public_GetMessageAndImportToChatbotAsync(IEnumerable<MessageSettlement> messageInfos, IEnumerable<MessageSettlementInfo> messageDetails)
        {
            await base.GetMessageAndImportToChatbotAsync(messageInfos, messageDetails);
        }

        public async Task<(IEnumerable<MessageSettlementHeader>, IEnumerable<MessageTarget>)> Public_GetEventInfoAsync(
            IEnumerable<MessageSettlement> messageInfo,
            IEnumerable<MessageSettlementInfo> messageDetail
        )
        {
            return await base.GetEventInfoAsync(messageInfo, messageDetail);
        }

        public async Task Public_ImportSettlementInternalCSDToChatbotDBAsync(IEnumerable<MessageSettlementHeader> messageHeaders, IEnumerable<MessageTarget> msgs)
        {
            await base.ImportSettlementInternalCSDToChatbotDBAsync(messageHeaders, msgs);
        }

        public async Task Public_GetMessageLicenseeAndImportToChatbotAsync(
            IEnumerable<MessageSettlement> messageInfos,
            IEnumerable<MessageSettlementDetail> messageDetails,
            IEnumerable<MessageSettlementInfo> messageTargets)
        {
            await base.GetMessageLicenseeAndImportToChatbotAsync(messageInfos, messageDetails, messageTargets);
        }

        public async Task<IEnumerable<MessageLeagueLicensee>> Public_GetLeagueAsync(IEnumerable<MessageSettlement> messageInfo)
        {
            return await base.GetLeagueAsync(messageInfo);
        }

        public async Task Public_ImportSettlementLicenseeToChatbotDBAsync(
            IEnumerable<MessageLeagueLicensee> messageHeaders,
            IEnumerable<MessageSettlementDetail> messageDetails,
            IEnumerable<MessageSettlementInfo> messageTargets
        )
        {
            await base.ImportSettlementLicenseeToChatbotDBAsync(messageHeaders, messageDetails, messageTargets);
        }
    }
}
