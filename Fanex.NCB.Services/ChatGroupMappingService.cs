namespace Fanex.NCB.Services
{
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IChatGroupMappingRepository chatGroupMappingRepository;

        public ChatGroupMappingService(IChatGroupMappingRepository chatGroupMappingRepository)
        {
            this.chatGroupMappingRepository = chatGroupMappingRepository;
        }

        public async Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model)
        {
            return await chatGroupMappingRepository.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditAsync(EditChatGroupMapping model)
        {
            return await chatGroupMappingRepository.EditAsync(model).ConfigureAwait(false);
        }
    }
}