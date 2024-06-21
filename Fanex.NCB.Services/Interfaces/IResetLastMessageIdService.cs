namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Models;

    public interface IResetLastMessageIdService
    {
        Task<(IEnumerable<CurrentMaxMessageInfo>, IEnumerable<MessageTypeInfo>)> GetCurrentPreviousMaxMessageAsync();

        Task<IEnumerable<MessageRequestId>> GetAbnormalBetsMissingMessageAsync(IEnumerable<CurrentMaxMessageInfo> messageInfoList);

        Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageInfoAsync(
            IEnumerable<CurrentMaxMessageInfo> msgList,
            IEnumerable<MessageTypeInfo> contentTypeList,
            IEnumerable<MessageRequestId> abnormalMissingMsgList);

        Task<IEnumerable<MissingMesssageInfo>> GetMissingMessageDataAsync(IEnumerable<MissingMesssageInfo> missingMesssages);

        Task<CommandResult> CreateMissingMessageAsync(List<MissingInfoIns> listMsg);
    }
}