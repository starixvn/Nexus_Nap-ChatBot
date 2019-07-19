namespace Fanex.NCB.Repository.ChatGroupMapping
{
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.ChatGroupMapping;
    using System.Threading.Tasks;

    public interface IChatGroupMappingRepository
    {
        Task<CommandResult> Edit(EditChatGroupMapping model);
    }
}