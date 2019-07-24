namespace Fanext.NCB.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;

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