using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMapping
{
    public interface IChatGroupMappingService
    {
        Task<List<BotName>> GetBotNameAsync();
        Task<List<ChatGroupType>> GetChatGroupTypeAsync();
        List<MappingStatus> GetMappingStatus();
        Task<ChatGroupMappingResultModel> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria);
        Task<ResultModel> UpdateChatGroupMappingAsync(EditChatGroupMappingCriteria criteria);
    }
}
