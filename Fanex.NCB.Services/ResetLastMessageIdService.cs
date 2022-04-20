namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class ResetLastMessageIdService : IResetLastMessageIdService
    {
        private readonly IBaseService baseService;
        private readonly string Environment;

        public ResetLastMessageIdService(IBaseService baseService, IConfiguration configuration)
        {
            this.baseService = baseService;
            Environment = configuration.GetSection("Environment")?.Value;
        }

        public async Task<(IEnumerable<CurrentMaxMessageInfo>, IEnumerable<MessageTypeInfo>)> GetCurrentPreviousMaxMessageAsync()
        {
            var criteria = new GetCurrentPreviousMaxMessageCriteria();
            return await baseService.FetchMultipleAsync<CurrentMaxMessageInfo, MessageTypeInfo>(criteria);
        }

        public async Task<(IEnumerable<MessagePostTime>, int)> GetNewPreviousMaxMessageAsync(GetNewPreviousMaxMessageCriteria criteria)
        {
            var (messagePostTimeInfos, newMaxMessageId) = await baseService.FetchMultipleAsync<MessagePostTime, int>(criteria);
            return (messagePostTimeInfos, newMaxMessageId.FirstOrDefault());
        }

        public async Task<IEnumerable<MessageRequestId>> GetMissingMessageCsdAsync(GetMissingMesssageCsdCriteria criteria)
        {
            return await baseService.GetDataAsync<MessageRequestId>(criteria);
        }

        public async Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageInfoAsync(GetMissingMessageInfoCriteria criteria)
        {
            return await baseService.GetDataAsync<MissingMesssageInfo>(criteria);
        }

        public async Task<(IEnumerable<FromToLogId>, IEnumerable<MissingMesssageInfo>)> GetMissingProblemAccountAsync(GetMissingProblemAccountCriteria criteria)
        {
            return await baseService.FetchMultipleAsync<FromToLogId, MissingMesssageInfo>(criteria);
        }

        public async Task<CommandResult> CreateMissingMessageAsync(CreateMissingMessageCommand command)
        {
            command.Environment = Environment;
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> EditMessageInfoAsync(EditMessageInfoCommand command)
        {
            command.JobInfo = JsonConvert.SerializeObject(command.JobInfoList);
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}
