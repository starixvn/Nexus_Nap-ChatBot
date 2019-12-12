namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Shared.Models;

    public interface IChatGroupMappingService
    {
        Task<CommandResult> EditAsync(EditChatGroupMapping model);

        Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model);

        Task<CommandResult> UpdateActiveAsync(UpdateActiveConversationGroup model);
    }
}