namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models;

    public interface ISyncDataService
    {
        Task NapUserControlSyncDataAsync();

        Task<CommandResult> NapSSOSyncDataByUserIdAsync(int userId);

        Task OicSyncDataAsync();
    }
}