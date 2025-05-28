namespace Fanex.NCB.AutoJobs.Core.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteria.MonitoredPlayer;
using Models.Common;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Models.MonitoredPlayer;
using Models.NotifictaionSendMessage;
using Interfaces;
using Newtonsoft.Json;

public class MonitoredPlayerService : IMonitoredPlayerService
{
    private readonly IBaseService baseService;
    private readonly IChatbotService chatbotService;
    private readonly ICommonService commonService;

    public MonitoredPlayerService(
        IBaseService baseService,
        IChatbotService chatbotService,
        ICommonService commonService
    )
    {
        this.baseService = baseService;
        this.chatbotService = chatbotService;
        this.commonService = commonService;
    }

    public async Task SendMessageMonitoredPlayerAsync()
    {
        var messageSetting = await GetMessageSettingAsync();
        if (messageSetting != null)
        {
            var (messages, messageSel) = await GetMonitoredPlayerAsync(messageSetting);

            if (messages.Any())
            {
                var (command, notifications) = await ImportMessageFromMonitoredPlayerAsync(messages,
                    messageSel.ServerName, messageSel.LastDataRow, messageSel.MaxLogId);
                await commonService.OnAfterImportDataAsync(command, notifications);
            }
        }
    }

    public async Task<MessageSetting> GetMessageSettingAsync()
    {
        var criteria = new GetMsgSettingMonitoredPlayerCriteria();
        return await baseService.GetDataEntityAsync<MessageSetting>(criteria);
    }

    public async Task<(IEnumerable<MonitoredPlayerInfo>, MessageSetting)> GetMonitoredPlayerAsync(
        MessageSetting messageSetting)
    {
        var criteria = new GetMonitoredPlayerInfoCriteria
        {
            BatchSize = messageSetting.BatchSize,
            LastLogId = messageSetting.LastLogId,
            LastDataRow = messageSetting.LastDataRow,
            ServerName = messageSetting.ServerName
        };
        var messageInfo = await baseService.GetDataAsync<MonitoredPlayerInfo>(criteria);
        var messageSel = new MessageSetting
        {
            LastDataRow = criteria.LastDataRow,
            ServerName = criteria.ServerName,
            MaxLogId = criteria.MaxLogId
        };
        return (messageInfo, messageSel);
    }

    public async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageFromMonitoredPlayerAsync(
        IEnumerable<MonitoredPlayerInfo> messages,
        string serverName,
        string lastDataRow,
        int pLastLogId
    )
    {
        var criteria = new ImportMsgFromMonitorMatchCriteria
        {
            Message = JsonConvert.SerializeObject(messages),
            ServerName = serverName,
            LastDataRow = lastDataRow,
            PLastLogId = pLastLogId
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