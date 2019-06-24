using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System.Collections.Generic;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService
{
    public interface IChatGroupMappingResultService
    {
        ChatGroupMappingResult BuildChatGroupMappingResult(BotConversation botConversation, BotConversationMember botConversationMember, List<ConversationRecipient> conversationRecipients);
    }
}