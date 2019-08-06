namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Core.Models;

    public interface IConversationService
    {
        Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command);

        Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command);

        Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command);

        Task<ConversationGroup> GetGroupConversation(string conversationId, string botName);
    }
}