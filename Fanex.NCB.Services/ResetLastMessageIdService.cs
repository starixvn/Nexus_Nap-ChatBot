namespace Fanex.NCB.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Criteria.ResetLastMessageId;
using Models.ResetLastMessageId;
using Interfaces;
using Shared.Enum;
using Shared.Extensions;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class ResetLastMessageIdService : IResetLastMessageIdService
{
    private readonly IBaseService baseService;
    private readonly ICommonService commonService;
    private readonly string Environment;

    public ResetLastMessageIdService(IBaseService baseService, IConfiguration configuration,
        ICommonService commonService)
    {
        this.baseService = baseService;
        Environment = configuration.GetSection("Environment")?.Value;
        this.commonService = commonService;
    }

    public async Task<(IEnumerable<CurrentMaxMessageInfo>, IEnumerable<MessageTypeInfo>)>
        GetCurrentPreviousMaxMessageAsync()
    {
        const int ListNotificationOfNotificationCenterIndex = 10;
        var criteria = new GetCurrentPreviousMaxMessageCriteria();
        var (currentMaxMessageInfos, messageTypeInfos) = await baseService.FetchMultipleAsync<CurrentMaxMessageInfo, MessageTypeInfo>(criteria);

        return (currentMaxMessageInfos
                .Select((value, index) => new { value, index })
                .Where(x => x.index != ListNotificationOfNotificationCenterIndex)
                .Select(x => x.value), messageTypeInfos);
    }

    public async Task<(IEnumerable<MessageRequestId>, IEnumerable<int>)> GetAbnormalBetsMissingMessageAsync()
    {
        var criteria = new GetMissingMesssageCsdCriteria();
        return await baseService.FetchMultipleAsync<MessageRequestId, int>(criteria);
    }

    public async Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageDataAsync(
        IEnumerable<MissingMesssageInfo> missingMesssages)
    {
        foreach (var item in missingMesssages)
        {
            item.Content_en = item.Content;
        }
        var criteria = new GetMissingMessageDataCriteria
        {
            MessageList = XmlHelper<MissingMesssageInfo>.Serialize(missingMesssages.ToList())
        };
        return await baseService.GetDataAsync<MissingMesssageInfo>(criteria);
    }

    public async Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageInfoAsync(
        IEnumerable<CurrentMaxMessageInfo> msgList,
        IEnumerable<MessageTypeInfo> contentTypeList,
        IEnumerable<MessageRequestId> abnormalMissingMsgList,
        IEnumerable<int> messageIds)
    {
        var criteria = new GetMissingMessageInfoCriteria
        {
            SiteAsJson = await commonService.GetJsonDataFromDbACCAsync("Site"),
            DepCustSAsJson = await commonService.GetJsonDataFromDbACCAsync("Dep_CustSuper"),
            Messages = msgList.Where(m =>
                    m.JobIdentify != (int)JobIdentity.ProblemPlayer &&
                    m.JobIdentify != (int)JobIdentity.MonitoredPlayer &&
                    m.JobIdentify != (int)JobIdentity.MonitorMatch)
                .ToList(),
            MsgContentTypes = contentTypeList.Where(m =>
                    m.JobIdentify != (int)JobIdentity.ProblemPlayer &&
                    m.JobIdentify != (int)JobIdentity.MonitoredPlayer &&
                    m.JobIdentify != (int)JobIdentity.MonitorMatch)
                .ToList(),
            Requests = abnormalMissingMsgList.ToList(),
            MessageIds = messageIds.ToList()
        };
        var missingMsg = await baseService.GetDataAsync<MissingMesssageInfo>(criteria);
        var missingInfo = await GetMissingMessageDataAsync(missingMsg);

        var problemAccountFromLogId = GetIdentifyFromLogId(msgList, JobIdentity.ProblemPlayer);
        var problemMissingInfo = await GetMissingProblemAccountAsync(problemAccountFromLogId);
        missingInfo = missingInfo.Concat(problemMissingInfo);

        var monitoredPlayerFromLogId = GetIdentifyFromLogId(msgList, JobIdentity.MonitoredPlayer);
        var monitoredMissingInfo = await GetMissingMonitoredPlayerAsync(monitoredPlayerFromLogId);
        missingInfo = missingInfo.Concat(monitoredMissingInfo);

        var matchMonitoring = GetIdentifyFromLogId(msgList, JobIdentity.MonitorMatch);
        var matchMonirotingMissingInfo = await GetMissingMonitoringMatchAsync(matchMonitoring);
        missingInfo = missingInfo.Concat(matchMonirotingMissingInfo);

        var messageMissingInfo = await GetMissingMessageNotificationAsync(msgList, missingInfo);

        CalculateMaxReferrenceId(msgList, messageMissingInfo);

        return messageMissingInfo;
    }

    private static string GetIdentifyFromLogId(IEnumerable<CurrentMaxMessageInfo> msgList, JobIdentity jobIdentity)
         => msgList.FirstOrDefault(m => m.JobIdentify == (int)jobIdentity)?.ReferenceValue
            ?? throw new ArgumentNullException(JsonConvert.SerializeObject(msgList));

    private static void CalculateMaxReferrenceId(IEnumerable<CurrentMaxMessageInfo> msgList,
        IEnumerable<MissingMesssageInfo> missingInfo)
    {
        foreach (var item in msgList)
        {
            var jobMissedInfo = missingInfo.Where(m => m.JobIdentify == item.JobIdentify).ToList();
            var maxReferrenceIdOnList = jobMissedInfo.Any()
                ? GetMaxReferrenceIdOnListMissingMessageInfo(jobMissedInfo, item.JobIdentify, item.SettingName)
                : "0";
            var maxRefenceId = maxReferrenceIdOnList != "0" ? maxReferrenceIdOnList : item.ReferenceValue;
            item.MaxReferenceId = maxRefenceId;
        }
    }

    private static string GetMaxReferrenceIdOnListMissingMessageInfo(IEnumerable<MissingMesssageInfo> missingInfo,
        int jobIdentify, string settingName)
    {
        var maxRefenceId = missingInfo.Max(x => GetIdByJobIdentify(jobIdentify, x, settingName));

        if ((JobIdentity)jobIdentify == JobIdentity.ResultAmendmentInternalCSD && maxRefenceId is DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        return maxRefenceId.ToString();
    }

#pragma warning disable S1172 // Unused method parameters should be removed

    private static object GetIdByJobIdentify(int jobIdentify, MissingMesssageInfo item, string settingName)
    {
        switch ((JobIdentity)jobIdentify)
        {
            case JobIdentity.InternalCSD when settingName == "LastMsgTargetId":
                return item.MsgTargetId;

            case JobIdentity.InternalCSD:
                return item.VoidingHistoryCollectionId;

            case JobIdentity.SettlementInternalCSD:
            case JobIdentity.SettlementLicensee:
                return item.MsgTargetId;

            case JobIdentity.ResultAmendmentLicensee:
                return item.MsgTargetId;

            case JobIdentity.ResultAmendmentInternalCSD when settingName == "LastModifiedTime":
                return item.PostTime;

            default:
                return item.MessageId;
        }
    }

#pragma warning restore S1172 // Unused method parameters should be removed

    private async Task<IEnumerable<MissingMesssageInfo>> GetMissingProblemAccountAsync(string fromLogId)
    {
        var criteria = new GetMissingProblemAccountCriteria
        {
            FromLogId = fromLogId
        };
        var result = await baseService.GetDataAsync<MissingMesssageInfo>(criteria);

        foreach (var item in result)
        {
            item.JobIdentify = (int)JobIdentity.ProblemPlayer;
        }
        return result;
    }

    private async Task<IEnumerable<MissingMesssageInfo>> GetMissingMonitoredPlayerAsync(string fromLogId)
    {
        var criteria = new GetMissingMonitoredPlayerCriteria
        {
            FromLogId = fromLogId
        };
        var result = await baseService.GetDataAsync<MissingMesssageInfo>(criteria);

        foreach (var item in result)
        {
            item.JobIdentify = (int)JobIdentity.MonitoredPlayer;
        }
        return result;
    }

    private async Task<IEnumerable<MissingMesssageInfo>> GetMissingMonitoringMatchAsync(string fromLogId)
    {
        var criteria = new GetMissingMonitoringMatchCriteria
        {
            FromLogId = fromLogId
        };
        var result = await baseService.GetDataAsync<MissingMesssageInfo>(criteria);

        foreach (var item in result)
        {
            item.JobIdentify = (int)JobIdentity.MonitorMatch;
        }
        return result;
    }

    private async Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageNotificationAsync(
        IEnumerable<CurrentMaxMessageInfo> msgList, IEnumerable<MissingMesssageInfo> missingMesssages)
    {
        foreach (var item in missingMesssages)
        {
            item.Content_en = item.Content;
        }
        var criteria = new GetMissingMessageNotificationCriteria
        {
            Setting = XmlHelper<CurrentMaxMessageInfo>.Serialize(msgList.ToList()),
            MessageList = XmlHelper<MissingMesssageInfo>.Serialize(missingMesssages.ToList())
        };
        return await baseService.GetDataAsync<MissingMesssageInfo>(criteria);
    }

    public async Task<CommandResult> CreateMissingMessageAsync(List<MissingInfoIns> listMsg)
    {
        var command = new CreateMissingMessageCommand
        {
            SiteAsJson = await commonService.GetJsonDataFromDbACCAsync("Site"),
            DepCustSAsJson = await commonService.GetJsonDataFromDbACCAsync("Dep_CustSuper"),
            Environment = Environment,
            Messages = listMsg
        };
        var commandUpdateSystemSettings = await baseService.ExecuteNonQueryAsync(command);
        var commandResultMissing = await UpdateSystemSettingAsync(listMsg);

        return new CommandResult
        {
            Code = commandUpdateSystemSettings.Code + commandResultMissing.Code,
            Message = commandUpdateSystemSettings.Message + commandResultMissing.Message
        };
    }

    private async Task<CommandResult> UpdateSystemSettingAsync(List<MissingInfoIns> listMsg)
    {
        var jobInfos = listMsg.Select(i => new
        {
            i.JobIdentify,
            ReferenceValue = i.MessageId,
            i.SettingName
        }).ToList();

        var jobDetails = listMsg.Select(j => new
        {
            j.JobIdentify,
            j.NotificationId
        }).Where(j => (JobIdentity)j.JobIdentify == JobIdentity.ResultAmendmentInternalCSD).ToList();

        var command = new EditMessageInfoCommand
        {
            JobInfo = JsonConvert.SerializeObject(jobInfos),
            JobDetail = JsonConvert.SerializeObject(jobDetails)
        };
        return await baseService.ExecuteNonQueryAsync(command);
    }
}