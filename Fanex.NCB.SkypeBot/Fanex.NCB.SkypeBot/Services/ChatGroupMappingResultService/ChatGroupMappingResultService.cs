using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService
{
    public class ChatGroupMappingResultService : IChatGroupMappingResultService
    {
        private readonly IMappingStatusService _mappingStatusService;

        public ChatGroupMappingResultService(IMappingStatusService mappingStatusService)
        {
            _mappingStatusService = mappingStatusService;
        }

        public ChatGroupMappingResult BuildChatGroupMappingResult(BotConversation botConversation, BotConversationMember botConversationMember, List<ConversationRecipient> conversationRecipients)
        {
            var mappingStatusID = _mappingStatusService.CalculateMappingStatus(botConversation.IsMapped, botConversationMember);
            return new ChatGroupMappingResult
            {
                BotName = botConversationMember?.BotName,
                GroupID = botConversation.GroupID,
                ConversationID = botConversation.ConversationID,
                GroupName = botConversation.GroupName,
                ChatGroupTypeID = botConversation.ChatGroupTypeID,
                ChatGroupTypeName = botConversation.ChatGroupTypeName,
                RecipientTypeID = botConversation.RecipientTypeId,
                RecipientTypeName = botConversation.RecipientTypeName,
                RecipientIDs = GetRecipientIDs(conversationRecipients),
                RecipientNames = GetRecipientNames(conversationRecipients),
                RecipientStatusNames = GetRecipientStatusNames(conversationRecipients),
                MappingStatusID = mappingStatusID,
                MappingStatusName = _mappingStatusService.GetMappingStatusName(mappingStatusID),
                Remark = botConversation.Remark
            };
        }

        private static string GetRecipientIDs(List<ConversationRecipient> conversationRecipients)
        {
            return String.Join(",", conversationRecipients.Select(x => x.RecipientID).ToList());
        }

        private static string GetRecipientNames(List<ConversationRecipient> conversationRecipients)
        {
            return String.Join(", ", conversationRecipients.Select(x => x.RecipientName).ToList());
        }

        private static string GetRecipientStatusNames(List<ConversationRecipient> conversationRecipients)
        {
            return String.Join(", ", conversationRecipients.Where(x => !String.IsNullOrEmpty(x.RecipientStatusName)).Select(x => x.RecipientStatusName).ToList());
        }
    }
}
