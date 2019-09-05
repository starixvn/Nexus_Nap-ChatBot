namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISyncDataService
    {
        Task NapUserControlSyncDataAsync();

        Task SyncNapDataForSSOLogin(int userId);

        Task OicSyncDataAsync();
    }
}