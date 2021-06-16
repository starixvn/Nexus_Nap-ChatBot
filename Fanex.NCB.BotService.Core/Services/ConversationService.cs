namespace Fanex.NCB.BotService.Core.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class ConversationService : IConversationService
    {
        private readonly IBaseService baseService;

        public ConversationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> AddMessageLog(AddMessageLogCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}