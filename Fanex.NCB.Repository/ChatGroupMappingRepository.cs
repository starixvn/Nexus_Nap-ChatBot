namespace Fanex.NCB.Repository
{
    using System.Threading.Tasks;
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Base.Services.Interfaces;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.Interfaces;

    public class ChatGroupMappingRepository : IChatGroupMappingRepository
    {
        private IBaseService baseService;

        public ChatGroupMappingRepository(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> Edit(EditChatGroupMapping model)
        {
            return await baseService.ExecuteNonQueryAsync(new EditChatGroupMappingCommand()).ConfigureAwait(false);
        }
    }
}