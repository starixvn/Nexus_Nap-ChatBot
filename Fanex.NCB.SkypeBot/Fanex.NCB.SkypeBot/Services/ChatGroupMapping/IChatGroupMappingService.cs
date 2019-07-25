using System.Threading.Tasks;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMapping
{
    public interface IChatGroupMappingService
    {
        Task<ChatGroupMappingResultModel> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria);

        Task<ResultModel> UpdateChatGroupMappingAsync(EditChatGroupMappingCriteria criteria);
    }
}