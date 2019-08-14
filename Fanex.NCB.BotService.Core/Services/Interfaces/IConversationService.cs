namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.Shared.Models;
    using System.Threading.Tasks;

    public interface IConversationService
    {
        Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command);

        Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command);

        Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command);

        Task<ConversationGroup> GetGroupConversation(string conversationId, string botName);

        Task<CommandResult> AddMessageLog(AddMessageLogCommand command);
    }
}