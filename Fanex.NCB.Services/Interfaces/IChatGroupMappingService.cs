namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Models.ChatGroupMapping;

    public interface IChatGroupMappingService
    {
        Task<CommandResult> Edit(EditChatGroupMapping model);
    }
}