namespace Fanex.NCB.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;

    public interface IChatGroupMappingService
    {
        Task<CommandResult> Update(EditChatGroupMapping model);

        Task<ValueTuple<IEnumerable<ChatGroupMappingResult>, long>> GetChatGroupMappingListAsync(GetChapGroupMapping model);
    }
}