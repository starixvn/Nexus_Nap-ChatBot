using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface IChatGroupMappingStorage
    {
        Task<Tuple<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria);

        Task<ResultModel> EditChatGroupMappingAsync(EditChatGroupMappingCriteria item);
    }
}