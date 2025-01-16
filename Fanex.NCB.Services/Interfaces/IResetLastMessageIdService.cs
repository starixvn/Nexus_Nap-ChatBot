namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Models;

    public interface IResetLastMessageIdService
    {
        Task<(IEnumerable<CurrentMaxMessageInfo>, IEnumerable<MessageTypeInfo>)> GetCurrentPreviousMaxMessageAsync();

        Task<(IEnumerable<MessageRequestId>, IEnumerable<int>)> GetAbnormalBetsMissingMessageAsync();

        Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageInfoAsync(
            IEnumerable<CurrentMaxMessageInfo> msgList,
            IEnumerable<MessageTypeInfo> contentTypeList,
            IEnumerable<MessageRequestId> abnormalMissingMsgList,
            IEnumerable<int> messageIds);

        Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageDataAsync(IEnumerable<MissingMesssageInfo> missingMesssages);

        Task<CommandResult> CreateMissingMessageAsync(List<MissingInfoIns> listMsg);
    }
}