namespace Fanex.NCB.Repository.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.ChatGroupMapping;

    public interface IChatGroupMappingRepository
    {
        Task<CommandResult> Edit(EditChatGroupMapping model);

        Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model);
    }
}