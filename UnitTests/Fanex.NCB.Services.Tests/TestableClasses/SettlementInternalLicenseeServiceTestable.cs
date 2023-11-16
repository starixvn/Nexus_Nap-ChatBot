namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
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
    }
}
