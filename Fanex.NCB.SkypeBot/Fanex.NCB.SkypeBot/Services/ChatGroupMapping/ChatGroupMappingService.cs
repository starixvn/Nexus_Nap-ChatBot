using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMapping
{
    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IChatGroupMappingStorage _chatGroupMappingStorage;
        private readonly IChatGroupMappingResultService _chatGroupMappingResultService;

        public ChatGroupMappingService(IChatGroupMappingStorage chatGroupMappingStorage, IChatGroupMappingResultService chatGroupMappingResultService)
        {
            _chatGroupMappingStorage = chatGroupMappingStorage;
            _chatGroupMappingResultService = chatGroupMappingResultService;
        }        

        public async Task<ChatGroupMappingResultModel> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            var data = await _chatGroupMappingStorage.GetChatGroupMappingListAsync(criteria);
            var count = data.Item1;
            var conversationList = data.Item2;
            var conversationMemberList = data.Item3;
            var conversationRecipientList = data.Item4;

            var result = new ChatGroupMappingResultModel
            {
                Count = count,
                Data = conversationList
                    .Select(botConversation =>
                    {
                        var botConversationMember = GetConversationMember(conversationMemberList, botConversation.ConversationID);
                        var conversationRecipients = GetConversationRecipients(conversationRecipientList, botConversation.GroupID);
                        return _chatGroupMappingResultService.BuildChatGroupMappingResult(botConversation, botConversationMember, conversationRecipients);                   
                    })
                    .ToList()
            };

            return result;
        }        

        public Task<ResultModel> UpdateChatGroupMappingAsync(EditChatGroupMappingCriteria criteria)
        {
            return _chatGroupMappingStorage.EditChatGroupMappingAsync(criteria);
        }

        private static List<ConversationRecipient> GetConversationRecipients(List<ConversationRecipient> conversationRecipientList, int groupID)
        {
            return conversationRecipientList.Where(i => i.GroupID == groupID).ToList();
        }       

        private static BotConversationMember GetConversationMember(List<BotConversationMember> conversationMemberList, string conversationID)
        {
            return conversationMemberList.FirstOrDefault(i => i.ConversationID == conversationID);
        }        
    }
}
