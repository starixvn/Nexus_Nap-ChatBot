namespace Fanex.NCB.AutoJobs.Core.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteria.SiteMessage;
using Enum;
using Helpers;
using Models.Common;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Models.NotifictaionSendMessage;
using Models.SiteMessage;
using Models.SpecialDeleteTicket;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class SiteMessageService : ISiteMessageService
{
    private readonly IBaseService baseService;
    private readonly IPushSignalRService pushSignalService;
    private readonly IChatbotService chatbotService;
    private readonly ICommonService commonService;
    private readonly string Environment;
    private const int SiteMessageJobIdentify = 3;

    public SiteMessageService(
        IBaseService baseService,
        IChatbotService chatbotService,
        IPushSignalRService pushSignalService,
        IConfiguration configuration,
        ICommonService commonService)
    {
        this.baseService = baseService;
        this.pushSignalService = pushSignalService;
        this.chatbotService = chatbotService;
        Environment = configuration.GetSection("Environment")?.Value;
        this.commonService = commonService;
    }

    public async Task PullMessageFromSiteMessageAsync()
    {
        var systemSetting = await baseService.GetDataEntityAsync<SystemSetting>(new GetSiteMessageSettingCriteria());
        if (systemSetting != null)
        {
            var (messages, messageDetails) = await GetDataFromSiteMessageAsync(systemSetting);
            var matchInfos = await GetMatchInfoAsync(string.Join(",", messages.Select(m => m.MatchID).Distinct()));

            MappingData(messages, matchInfos);
            if (messages.Any() && messageDetails.Any())
            {
                var (command, notifications) = await ImportMessageToChatbotDBAsync(messages, messageDetails);
                await OnAfterImportDataAsync(command, notifications, messageDetails);
            }
        }
    }

    protected internal virtual async Task<(CommandResult, IEnumerable<NotificationMessage>)>
        ImportMessageToChatbotDBAsync(
            IEnumerable<SiteMessageModel> messages,
            IEnumerable<SiteMessageDetail> messageDetails)
    {
        var criteria = new ImportDataSiteMessageAndGetGroupTestCriteria
        {
            MessageHeader = JsonConvert.SerializeObject(messages),
            MessageDetail = JsonConvert.SerializeObject(messageDetails)
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

    protected internal virtual async Task OnAfterImportDataAsync(
        CommandResult commandResult,
        IEnumerable<NotificationMessage> notifications,
        IEnumerable<SiteMessageDetail> messageDetails)
    {
        if (!commandResult.IsSuccessful)
        {
            await chatbotService.NotifyToChatGroupAsync(commandResult.Message);
        }
        else
        {
            if (notifications.Any())
            {
                await InsertMessageHistoryAsync(messageDetails);

                await Task.WhenAll(
                    commonService.SendMessageByChannelAsync((byte)ConversationChannel.Skype, notifications),
                    commonService.SendMessageByChannelAsync((byte)ConversationChannel.Telegram, notifications)
                );

                var notificationIds = string.Join(",", notifications.Select(item => item.NotificationId));
                await pushSignalService.PushNewNotification(notificationIds);
            }
        }
    }

    protected internal virtual async Task InsertMessageHistoryAsync(IEnumerable<SiteMessageDetail> messageDetails)
    {
        var msgHistory = messageDetails
            .Select(x => new MessageHistory { MessageId = x.MessageID, TargetId = x.TargetID }).ToList();
        var command = new InsertMessageHistoryCommand
        {
            JobIdentify = SiteMessageJobIdentify,
            MessageIDLst = XmlHelper<MessageHistory>.Serialize(msgHistory),
            Environment = Environment
        };

        await baseService.ExecuteNonQueryAsync(command);
    }

    private static void MappingData(IEnumerable<SiteMessageModel> messages,
        IEnumerable<SiteMessageMatchInfoModel> matchInfos)
    {
        foreach (var message in messages)
        {
            var matchInfo = matchInfos.FirstOrDefault(i => i.MatchID == message.MatchID);
            if (matchInfo != null)
            {
                message.SportType = matchInfo.SportType;
                message.SportNameEN = matchInfo.SportNameEN;
                message.SportNameCS = matchInfo.SportNameCS;
                message.KickOffTime = matchInfo.KickOffTime;
                message.IsTestMatch = matchInfo.IsTestMatch;
                message.OrigLeagueNameEN = matchInfo.LeagueNameEN;
                message.OrigLeagueNameCS = matchInfo.LeagueNameCS;
                message.LeagueNameEN = TelegramFormatExtensions.EscapeSpecialSyntax(matchInfo.LeagueNameEN);
                message.LeagueNameCS = TelegramFormatExtensions.EscapeSpecialSyntax(matchInfo.LeagueNameCS);
                message.HomeNameEN = TelegramFormatExtensions.EscapeSpecialSyntax(matchInfo.HomeNameEN);
                message.HomeNameCS = TelegramFormatExtensions.EscapeSpecialSyntax(matchInfo.HomeNameCS);
                message.AwayNameEN = TelegramFormatExtensions.EscapeSpecialSyntax(matchInfo.AwayNameEN);
                message.AwayNameCS = TelegramFormatExtensions.EscapeSpecialSyntax(matchInfo.AwayNameCS);
                message.IsTest = matchInfo.IsTestMatch || message.IsTestMsg;
            }
        }
    }

    protected internal virtual async Task<IEnumerable<SiteMessageMatchInfoModel>> GetMatchInfoAsync(string matchIds)
    {
        var criteria = new GetEventInfoCriteria
        {
            MatchIDLst = matchIds
        };
        return await baseService.GetDataAsync<SiteMessageMatchInfoModel>(criteria);
    }

    protected internal virtual async Task<(IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>)>
        GetDataFromSiteMessageAsync(SystemSetting systemSetting)
    {
        var criteria = new GetMessageInfoFromAdminSiteCriteria
        {
            SiteAsJson = await commonService.GetJsonDataFromDbACCAsync("Site"),
            MessageID = systemSetting.MaxMessageID,
            BatchSize = systemSetting.BatchSize,
            LastPostTime = systemSetting.LastPostTime,
            MsgContentsTypeLst = systemSetting.MsgContentsTypeLst,
            Environment = Environment
        };
        var (messages, messageDetails) =
            await baseService.FetchMultipleAsync<SiteMessageModel, SiteMessageDetail>(criteria);
        foreach (var msg in messages)
        {
            msg.Content_en = !string.IsNullOrWhiteSpace(msg.Content_en) ? msg.Content_en.Trim() : string.Empty;
            msg.Content_cs = !string.IsNullOrWhiteSpace(msg.Content_cs) ? msg.Content_cs.Trim() : string.Empty;
        }

        return (messages, messageDetails);
    }
}