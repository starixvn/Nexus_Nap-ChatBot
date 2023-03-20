namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Enum;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class ResetLastMessageIdService : IResetLastMessageIdService
    {
        private readonly IBaseService baseService;
        private readonly ILogService logService;
        private readonly string Environment;

        public ResetLastMessageIdService(IBaseService baseService, IConfiguration configuration, ILogService logService)
        {
            this.baseService = baseService;
            Environment = configuration.GetSection("Environment")?.Value;
            this.logService = logService;
        }

        public async Task<(IEnumerable<CurrentMaxMessageInfo>, IEnumerable<MessageTypeInfo>)> GetCurrentPreviousMaxMessageAsync()
        {
            var criteria = new GetCurrentPreviousMaxMessageCriteria();
            return await baseService.FetchMultipleAsync<CurrentMaxMessageInfo, MessageTypeInfo>(criteria);
        }

        public async Task<IEnumerable<MessageRequestId>> GetAbnormalBetsMissingMessageAsync(IEnumerable<CurrentMaxMessageInfo> messageInfoList)
        {
            var criteria = new GetMissingMesssageCsdCriteria
            {
                MessageList = XmlHelper<CurrentMaxMessageInfo>.Serialize(messageInfoList.ToList())
            };
            return await baseService.GetDataAsync<MessageRequestId>(criteria);
        }

        public async Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageInfoAsync(
            IEnumerable<CurrentMaxMessageInfo> msgList,
            IEnumerable<MessageTypeInfo> contentTypeList,
            IEnumerable<MessageRequestId> abnormalMissingMsgList)
        {
            try
            {
                var criteria = new GetMissingMessageInfoCriteria
                {
                    Messages = msgList.Where(m => m.JobIdentify != (int)JobIdentity.ProblemPlayer && m.JobIdentify != (int)JobIdentity.MonitoredPlayer).ToList(),
                    MsgContentTypes = contentTypeList.Where(m => m.JobIdentify != (int)JobIdentity.ProblemPlayer && m.JobIdentify != (int)JobIdentity.MonitoredPlayer).ToList(),
                    Requests = abnormalMissingMsgList.ToList()
                };
                var missingInfo = await baseService.GetDataAsync<MissingMesssageInfo>(criteria);
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(missingInfo));
                var problemAccountFromLogId = msgList.FirstOrDefault(m => m.JobIdentify == (int)JobIdentity.ProblemPlayer).ReferenceId;
                var problemMissingInfo = await GetMissingProblemAccountAsync(problemAccountFromLogId);
                missingInfo = missingInfo.Concat(problemMissingInfo);

                var monitoredPlayerFromLogId = msgList.FirstOrDefault(m => m.JobIdentify == (int)JobIdentity.MonitoredPlayer).ReferenceId;
                var monitoredMissingInfo = await GetMissingMonitoredPlayerAsync(monitoredPlayerFromLogId);
                missingInfo = missingInfo.Concat(monitoredMissingInfo);

                CalculateMaxReferrenceId(msgList, missingInfo);
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(msgList));
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(missingInfo));

                return missingInfo;
            }
            catch (System.Exception ex)
            {
                await logService.WriteExceptionSentryErrorAsync(ex);

                throw;
            }
        }

        private static void CalculateMaxReferrenceId(IEnumerable<CurrentMaxMessageInfo> msgList, IEnumerable<MissingMesssageInfo> missingInfo)
        {
            foreach (var item in msgList)
            {
                var jobMissedInfo = missingInfo.Where(m => m.JobIdentify == item.JobIdentify).ToList();
                int maxRefenceId = jobMissedInfo.Any()
                    ? GetMaxReferrenceIdOnListMissingMessageInfo(jobMissedInfo, item.JobIdentify)
                    : item.ReferenceId;
                item.MaxReferenceId = maxRefenceId;
            }
        }

        private static int GetMaxReferrenceIdOnListMissingMessageInfo(IEnumerable<MissingMesssageInfo> missingInfo, int jobIdentify)
            => missingInfo.Max(x => jobIdentify == (int)JobIdentity.InternalCSD ? x.VoidingHistoryCollectionId : x.MessageId);

        private async Task<IEnumerable<MissingMesssageInfo>> GetMissingProblemAccountAsync(int fromLogId)
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

        private async Task<IEnumerable<MissingMesssageInfo>> GetMissingMonitoredPlayerAsync(int fromLogId)
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

        public async Task<CommandResult> CreateMissingMessageAsync(List<MissingMesssageInfo> listMsg)
        {
            var command = new CreateMissingMessageCommand
            {
                Environment = Environment,
                Messages = listMsg
            };
            var commandUpdateSystemSettings = await baseService.ExecuteNonQueryAsync(command);
            var commandResultMissing = await UpdateSystemSettingAsync(listMsg);

            return new CommandResult
            {
                Code = commandUpdateSystemSettings.Code + commandResultMissing.Code,
                Message = commandUpdateSystemSettings.Message + commandResultMissing.Message,
            };
        }

        private async Task<CommandResult> UpdateSystemSettingAsync(List<MissingMesssageInfo> listMsg)
        {
            var jobLstSystemSettings = listMsg.Select(i => new
            {
                i.JobIdentify,
                ReferenceId = i.MessageId
            }).ToList();

            var command = new EditMessageInfoCommand
            {
                JobInfo = JsonConvert.SerializeObject(jobLstSystemSettings)
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}