namespace Fanex.NCB.RepositoryImpl.ChatGroupMapping
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.ChatGroupMapping;
    using System.Threading.Tasks;

    public class ChatGroupMappingRepository : BaseRepository, IChatGroupMappingRepository
    {
        public ChatGroupMappingRepository(IDynamicRepository dynamicRepository) : base(dynamicRepository)
        {
        }

        public async Task<CommandResult> Edit(EditChatGroupMapping model)
        {
            return await ExecuteNonQueryAsync(new EditChatGroupMappingCommand()).ConfigureAwait(false);
        }
    }
}