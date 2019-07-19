namespace Fanex.NCB.Services
{
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.ChatGroupMapping;
    using System.Threading.Tasks;

    public interface IChatGroupMappingService
    {
        Task<CommandResult> Edit(EditChatGroupMapping model);
    }
}