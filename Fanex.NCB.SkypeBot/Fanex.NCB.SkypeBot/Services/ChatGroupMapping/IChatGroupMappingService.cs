using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMapping
{
    public interface IChatGroupMappingService
    {
        Task<ChatGroupMappingResultModel> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria);

        Task<ResultModel> UpdateChatGroupMappingAsync(EditChatGroupMappingCriteria criteria);
    }
}