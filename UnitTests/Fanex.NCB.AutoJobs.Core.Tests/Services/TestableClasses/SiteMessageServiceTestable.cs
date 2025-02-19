namespace Fanex.NCB.AutoJobs.Core.Tests.Services.TestableClasses;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Models.NotifictaionSendMessage;
using Models.SiteMessage;
using Models.SpecialDeleteTicket;
using Fanex.NCB.AutoJobs.Core.Services;
using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;

public class SiteMessageServiceTestable : SiteMessageService
{
    public SiteMessageServiceTestable(
        IBaseService baseService,
        IChatbotService chatbotService,
        IPushSignalRService pushSignalService,
        IConfiguration configuration,
        ICommonService commonService)
        : base(baseService, chatbotService, pushSignalService, configuration, commonService)
    {
    }

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

    public async Task<(IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>)>
        Public_GetDataFromSiteMessageAsync(SystemSetting systemSetting)
    {
        return await base.GetDataFromSiteMessageAsync(systemSetting);
    }
}