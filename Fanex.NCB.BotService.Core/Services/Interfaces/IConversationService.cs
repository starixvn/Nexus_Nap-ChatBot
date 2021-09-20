namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.Shared.Models;

    public interface IConversationService
    {
        Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command);

        Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command);

        Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command);

        Task<CommandResult> MigratedToSupergroupAsync(MigratedToSupergroupCommand command);

        Task<CommandResult> AddMessageLog(AddMessageLogCommand command);

        IList<NotificationMessage> GetDistinctNotificationConversation(IList<NotificationMessage> notifications);
    }
}