namespace Fanex.NCB.Repository.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models.NapClient;
    using Fanex.NCB.Shared.Models.OIC;

    public interface ISyncDataRepository
    {
        Task<NapDataCrawler> FetchDataFromNapClient(ICriteria criteria);

        Task<OicDataCrawler> FetchDataFromOIC(ICriteria criteria);
    }
}