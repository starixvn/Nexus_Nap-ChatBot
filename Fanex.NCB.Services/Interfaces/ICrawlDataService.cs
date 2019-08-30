namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ICrawlDataService
    {
        Task NapClientSyncDataAsync();

        Task OicSyncDataAsync();
    }
}