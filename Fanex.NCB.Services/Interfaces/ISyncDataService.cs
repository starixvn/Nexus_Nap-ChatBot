namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISyncDataService
    {
        Task NapClientSyncDataAsync(int? userId);

        Task OicSyncDataAsync();
    }
}