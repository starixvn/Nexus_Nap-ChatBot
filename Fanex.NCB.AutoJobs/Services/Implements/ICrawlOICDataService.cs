namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System.Threading.Tasks;

    public interface ICrawlOicDataService
    {
        Task OicSyncDataAsync();
    }
}