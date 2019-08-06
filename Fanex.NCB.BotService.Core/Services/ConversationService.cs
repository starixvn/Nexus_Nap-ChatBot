namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Models;
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

        public async Task<ConversationGroup> GetGroupConversation(string conversationId, string botName)
        {
            var criteria = new GetConversationGroupCriteria
            {
                ConversationId = conversationId,
                BotName = botName
            };

            var conversationGroup = await baseService.GetDataEntityAsync<ConversationGroup>(criteria);
            return conversationGroup;
        }
    }
}