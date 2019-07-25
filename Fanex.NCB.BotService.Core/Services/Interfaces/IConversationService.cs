namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.BotService.Core.Criteria;
    using System.Threading.Tasks;

    public interface IConversationService
    {
        Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command);

        Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command);

        Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command);
    }
}