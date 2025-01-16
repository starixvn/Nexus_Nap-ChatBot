namespace Fanex.NCB.AutoJobs.Core.Tests.Services.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Settlement;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;

    public class SettlementInternalLicenseeServiceTestable : SettlementInternalLicenseeService
    {
        public SettlementInternalLicenseeServiceTestable(
            IBaseService baseService,
            IChatbotService chatbotService,
            ICommonService commonService
        ) : base(baseService, chatbotService, commonService)
        {
        }

        public async Task Public_GetMessageAndImportToChatbotAsync(IEnumerable<MessageSettlement> messageInfos, IEnumerable<MessageSettlementInfo> messageDetails, int maxMsgTargetId)
        {
            await base.GetMessageAndImportToChatbotAsync(messageInfos, messageDetails, maxMsgTargetId);
        }

        public async Task<(IEnumerable<MessageSettlementHeader>, IEnumerable<MessageTarget>)> Public_GetEventInfoAsync(
            IEnumerable<MessageSettlement> messageInfo,
            IEnumerable<MessageSettlementInfo> messageDetail
        )
        {
            return await base.GetEventInfoAsync(messageInfo, messageDetail);
        }

        public async Task Public_ImportSettlementInternalCSDToChatbotDBAsync(IEnumerable<MessageSettlementHeader> messageHeaders, IEnumerable<MessageTarget> msgs, int maxMsgTargetId)
        {
            await base.ImportSettlementInternalCSDToChatbotDBAsync(messageHeaders, msgs, maxMsgTargetId);
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
