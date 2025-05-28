namespace Fanex.NCB.AutoJobs.Core.Services;

using Fanex.NCB.AutoJobs.Core.Criteria.Common;
using Fanex.NCB.AutoJobs.Core.Criteria.MonitoredPlayer;
using Fanex.NCB.AutoJobs.Core.Models.Common;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Fanex.NCB.AutoJobs.Core.Models.MonitorMatch;
using Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;
using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
using Newtonsoft.Json;

public class MonitorMatchService : IMonitorMatchService
{
    private readonly IBaseService baseService;
    private readonly IChatbotService chatbotService;
    private readonly ICommonService commonService;
    private readonly ILogService logService;
    private readonly string MonitorMatchServiceName = "Match Monitoring";

    public MonitorMatchService(
        IBaseService baseService,
        IChatbotService chatbotService,
        ICommonService commonService,
        ILogService logService
    )
    {
        this.baseService = baseService;
        this.chatbotService = chatbotService;
        this.commonService = commonService;
        this.logService = logService;
    }

    public async Task SendMessageMonitoredMatchAsync()
    {
        var systemSetting = await GetSystemSetting();

        if (systemSetting is null)
        {
            await logService.WriteMessageSentryErrorAsync("Match Monitoring service setting is null.");
        }
        else
        {
            await logService.WriteMessageSentryErrorAsync($"Match Monitoring service setting is {JsonConvert.SerializeObject(systemSetting)}.");

            var (messages, output) = await GetMonitorMatchMessages(systemSetting);
            if (messages.Any())
            {
                var (command, notifications) = await ImportMsgMonitorMatchAsync(messages, output);
                await commonService.OnAfterImportDataAsync(command, notifications);
            }
        }

    }

    private async Task<MonitorMatchSetting> GetSystemSetting()
    {
        var criteria = new GetServiceSettingCriteria
        {
            InServiceName = MonitorMatchServiceName
        };

        return await baseService.GetDataEntityAsync<MonitorMatchSetting>(criteria);
    }

    private async Task<(IEnumerable<MonitorMatchInfo>, MessageSetting)> GetMonitorMatchMessages(MonitorMatchSetting systemSetting)
    {
        var criteria = new GetMsgSettingMonitorMatchCriteria
        {
            LastLogId = systemSetting.LastLogId,
            BatchSize = systemSetting.BatchSize,
            LastDataRow = systemSetting.LastDataRow,
            ServerName = systemSetting.ServerName
        };

        return (await baseService.GetDataAsync<MonitorMatchInfo>(criteria), new MessageSetting
        {
            LastDataRow = criteria.LastDataRow,
            ServerName = criteria.ServerName,
            MaxLogId = criteria.MaxLogId
        });
    }

    private async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMsgMonitorMatchAsync(
        IEnumerable<MonitorMatchInfo> messages, 
        MessageSetting output)
    {
        var criteria = new ImportMsgFromMonitorMatchCriteria
        {
            Message = JsonConvert.SerializeObject(messages),
            ServerName = output.ServerName,
            LastDataRow = output.LastDataRow,
            PLastLogId = output.MaxLogId
        };

        var (recipients, notificationMessages) =
            await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);

        var notifications =
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients);

        var command = new CommandResult
        {
            Code = criteria.ErrCode,
            Message = criteria.ErrMsg
        };

        return (command, notifications);
    }
}
