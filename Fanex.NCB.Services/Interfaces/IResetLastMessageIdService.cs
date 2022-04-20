namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Models;

    public interface IResetLastMessageIdService
    {
        Task<(IEnumerable<CurrentMaxMessageInfo>, IEnumerable<MessageTypeInfo>)> GetCurrentPreviousMaxMessageAsync();

        Task<(IEnumerable<MessagePostTime>, int)> GetNewPreviousMaxMessageAsync(GetNewPreviousMaxMessageCriteria criteria);

        Task<IEnumerable<MessageRequestId>> GetMissingMessageCsdAsync(GetMissingMesssageCsdCriteria criteria);

        Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageInfoAsync(GetMissingMessageInfoCriteria criteria);

        Task<(IEnumerable<FromToLogId>, IEnumerable<MissingMesssageInfo>)> GetMissingProblemAccountAsync(GetMissingProblemAccountCriteria criteria);

        Task<CommandResult> CreateMissingMessageAsync(CreateMissingMessageCommand command);

        Task<CommandResult> EditMessageInfoAsync(EditMessageInfoCommand command);
    }
}
