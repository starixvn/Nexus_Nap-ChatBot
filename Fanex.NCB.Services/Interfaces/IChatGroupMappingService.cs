namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.ChatGroupMapping;

    public interface IChatGroupMappingService
    {
        Task<CommandResult> EditAsync(EditChatGroupMapping model);

        Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model);
    }
}