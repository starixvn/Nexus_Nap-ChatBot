namespace Fanex.NCB.AutoJobs.Core.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants;
using Criteria.SettlementInternalCSD;
using Criteria.SettlementLicensee;
using Helpers;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Models.NotifictaionSendMessage;
using Models.Settlement;
using Interfaces;
using Newtonsoft.Json;

public class SettlementInternalLicenseeService : ISettlementInternalLicenseeService
{
    private readonly IBaseService baseService;
    private readonly IChatbotService chatbotService;
    private readonly ICommonService commonService;
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    public SettlementInternalLicenseeService
    (
        IBaseService baseService,
        IChatbotService chatbotService,
        ICommonService commonService
    )
    {
        this.baseService = baseService;
        this.chatbotService = chatbotService;
        this.commonService = commonService;
    }

    #region Settlement Internal CSD

    public async Task SettlementInternalCSDAsync()
    {
        var systemSetting = await commonService.GetServiceSetting(Constants.ServiceName_SettlementInternalCSD);

        if (systemSetting != null)
        {
            var criteria = new GetMessageSettleInternalCSDCriteria
            {
                LastMsgTargetId = systemSetting.LastMsgTargetId,
                BatchSize = systemSetting.BatchSize
            };

            var (messageInfos, messageDetails) =
                await baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementInfo>(criteria);
            await GetMessageAndImportToChatbotAsync(messageInfos, messageDetails, criteria.MaxMsgTargetId);
        }
    }

    protected internal virtual async Task GetMessageAndImportToChatbotAsync(
        IEnumerable<MessageSettlement> messageInfos,
        IEnumerable<MessageSettlementInfo> messageDetails,
        int maxMsgTargetId
    )
    {
        var (messageHeaders, msgs) = await GetEventInfoAsync(messageInfos, messageDetails);
        FormatTelegram(messageHeaders);
        await ImportSettlementInternalCSDToChatbotDBAsync(messageHeaders, msgs, maxMsgTargetId);
    }

    private static void FormatTelegram(IEnumerable<MessageSettlementHeader> messageHeaders)
    {
        foreach (var item in messageHeaders)
            item.SportNameEN = TelegramFormatExtensions.EscapeSpecialSyntax(item.SportNameEN);
    }

    protected internal virtual async Task<(IEnumerable<MessageSettlementHeader>, IEnumerable<MessageTarget>)>
        GetEventInfoAsync(
            IEnumerable<MessageSettlement> messageInfo,
            IEnumerable<MessageSettlementInfo> messageDetail
        )
    {
        var criteria = new GetEventSettleInternalCSDCriteria
        {
            MessageInfo = XmlHelper<MessageSettlement>.Serialize(messageInfo.ToList()),
            MessageDetail = XmlHelper<MessageSettlementInfo>.Serialize(messageDetail.ToList())
        };

        return await baseService.FetchMultipleAsync<MessageSettlementHeader, MessageTarget>(criteria);
    }

    protected internal virtual async Task ImportSettlementInternalCSDToChatbotDBAsync(
        IEnumerable<MessageSettlementHeader> messageHeaders,
        IEnumerable<MessageTarget> msgs,
        int maxMsgTargetId
    )
    {
        var criteria = new ImportSettleInternalCSDCommand
        {
            MessageHeader = JsonConvert.SerializeObject(messageHeaders,
                new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
            MessageDetail =
                JsonConvert.SerializeObject(msgs, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
            InMaxMsgTargetId = maxMsgTargetId
        };
        var (recipients, notifications) =
            await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
        var notificationMessages =
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);
        var command = new CommandResult
        {
            Code = criteria.ErrCode,
            Message = criteria.ErrMsg
        };

        await commonService.OnAfterImportDataAsync(command, notificationMessages);
    }

    #endregion Settlement Internal CSD

    #region Settlement Licensee

    public async Task SettlementLicenseeAsync()
    {
        var systemSetting = await commonService.GetServiceSetting(Constants.ServiceName_SettlementLicensee);

        if (systemSetting != null)
        {
            var criteria = new GetMessageSettlementLicenseeCriteria
            {
                LastMsgTargetId = systemSetting.LastMsgTargetId,
                BatchSize = systemSetting.BatchSize,
                SiteAsJSON = await commonService.GetJsonDataFromDbACCAsync("Site"),
                DepCustSAsJSON = await commonService.GetJsonDataFromDbACCAsync("Dep_CustSuper")
            };

            var (messageInfos, messageDetails, messageTargets) = await baseService
                .FetchMultipleAsync<MessageSettlement, MessageSettlementDetail, MessageSettlementInfo>(criteria);
            await GetMessageLicenseeAndImportToChatbotAsync(messageInfos, messageDetails, messageTargets);
        }
    }

    protected internal virtual async Task GetMessageLicenseeAndImportToChatbotAsync(
        IEnumerable<MessageSettlement> messageInfos,
        IEnumerable<MessageSettlementDetail> messageDetails,
        IEnumerable<MessageSettlementInfo> messageTargets
    )
    {
        var messageHeaders = await GetLeagueAsync(messageInfos);
        await ImportSettlementLicenseeToChatbotDBAsync(messageHeaders, messageDetails, messageTargets);
    }

    protected internal virtual async Task<IEnumerable<MessageLeagueLicensee>> GetLeagueAsync(
        IEnumerable<MessageSettlement> messageInfo
    )
    {
        var criteria = new GetLeagueSettlementLicenseeCriteria
        {
            Message = XmlHelper<MessageSettlement>.Serialize(messageInfo.ToList())
        };

        return await baseService.GetDataAsync<MessageLeagueLicensee>(criteria);
    }

    protected internal virtual async Task ImportSettlementLicenseeToChatbotDBAsync(
        IEnumerable<MessageLeagueLicensee> messageHeaders,
        IEnumerable<MessageSettlementDetail> messageDetails,
        IEnumerable<MessageSettlementInfo> messageTargets
    )
    {
        if (messageHeaders.Any())
        {
            var criteria = new ImportSettlementLicenseeCommand
            {
                MessageHeader = JsonConvert.SerializeObject(messageHeaders,
                    new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                MessageDetail = JsonConvert.SerializeObject(messageDetails,
                    new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                MessageTarget = JsonConvert.SerializeObject(messageTargets,
                    new JsonSerializerSettings { DateFormatString = DateTimeFormat })
            };
            var (recipients, notifications) =
                await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
            var notificationMessages =
                chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);
            var command = new CommandResult
            {
                Code = criteria.ErrCode,
                Message = criteria.ErrMsg
            };

            await commonService.OnAfterImportDataAsync(command, notificationMessages);
        }
    }

    #endregion Settlement Licensee
}