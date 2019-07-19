namespace Fanext.NCB.ServiceImpl.ChatGroupMapping
{
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.ChatGroupMapping;
    using Fanex.NCB.Services;
    using System.Threading.Tasks;

    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IChatGroupMappingRepository chatGroupMappingRepository;

        public ChatGroupMappingService(IChatGroupMappingRepository chatGroupMappingRepository)
        {
            this.chatGroupMappingRepository = chatGroupMappingRepository;
        }

        public async Task<CommandResult> Edit(EditChatGroupMapping model)
        {
            return await chatGroupMappingRepository.Edit(model).ConfigureAwait(false);
        }
    }
}