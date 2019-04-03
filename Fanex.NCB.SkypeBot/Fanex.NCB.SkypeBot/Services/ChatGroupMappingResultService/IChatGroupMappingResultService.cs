using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService
{
    public interface IChatGroupMappingResultService
    {
        ChatGroupMappingResult BuildChatGroupMappingResult(BotConversation botConversation, BotConversationMember botConversationMember, List<ConversationRecipient> conversationRecipients);
    }
}
