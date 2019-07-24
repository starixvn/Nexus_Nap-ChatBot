namespace Fanex.NCB.Repository.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Models.ChatGroupMapping;

    public interface IChatGroupMappingRepository
    {
        Task<CommandResult> Edit(EditChatGroupMapping model);
    }
}