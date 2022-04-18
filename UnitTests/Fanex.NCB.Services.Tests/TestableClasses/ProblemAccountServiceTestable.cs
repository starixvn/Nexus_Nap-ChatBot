namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class ProblemAccountServiceTestable : ProblemAccountService
    {
        public ProblemAccountServiceTestable(
            IBaseService baseService,
            ICommonService commonService,
            IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            ILogService logService
        ) : base(baseService, commonService, chatbotService, pushSignalService, logService)
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
