namespace Fanex.NCB.BotService.Core.Services
{
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Base.Services.Interfaces;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using System.Threading.Tasks;

    public class ConversationService : IConversationService
    {
        private readonly IBaseService baseService;

        public ConversationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command)
        {
            //return await baseService.ExecuteNonQueryAsync(command);
            return null;
        }

        public async Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command)
        {
            //return await baseService.ExecuteNonQueryAsync(command);
            return null;
        }

        public async Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command)
        {
            //return await baseService.ExecuteNonQueryAsync(command);
            return null;
        }
    }
}