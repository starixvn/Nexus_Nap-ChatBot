namespace Fanex.NCB.Repository.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models.NapClient;

    public interface ICrawlDataRepository
    {
        Task<NapDataCrawler> FetchDataFromNapClient(ICriteria criteria);
    }
}