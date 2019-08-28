namespace Fanex.NCB.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.CrawlData;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models.NapClient;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class CrawlDataService : ICrawlDataService
    {
        private readonly IOptionsSnapshot<AppSettings> appSettings;
        private readonly ICrawlDataRepository crawlDataRepository;
        private readonly IBaseService baseService;

        public CrawlDataService(IOptionsSnapshot<AppSettings> appSettings
            , ICrawlDataRepository crawlDataRepository,
            IBaseService baseService)
        {
            this.appSettings = appSettings;
            this.crawlDataRepository = crawlDataRepository;
            this.baseService = baseService;
        }

        public async Task NapClientSyncDataAsync()
        {
            var data = await GetDataFromNapClient().ConfigureAwait(false);
            await InsertNapClientData(data).ConfigureAwait(false);
        }

        private async Task<NapDataCrawler> GetDataFromNapClient()
        {
            var criteria = new GetDataFromNapClientCriteria
            {
                SiteId = appSettings.Value.SiteId
            };

            return await crawlDataRepository.FetchDataFromNapClient(criteria).ConfigureAwait(false);
        }

        private async Task InsertNapClientData(NapDataCrawler data)
        {
            var command = new InsertNapClientDataCommand
            {
                Data = JsonConvert.SerializeObject(data)
            };

            await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }
    }
}