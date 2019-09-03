namespace Fanex.NCB.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.CrawlData;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models.NapClient;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class SyncDataService : ISyncDataService
    {
        private readonly IOptionsSnapshot<AppSettings> appSettings;
        private readonly ISyncDataRepository crawlDataRepository;
        private readonly IBaseService baseService;

        public SyncDataService(IOptionsSnapshot<AppSettings> appSettings
            , ISyncDataRepository crawlDataRepository,
            IBaseService baseService)
        {
            this.appSettings = appSettings;
            this.crawlDataRepository = crawlDataRepository;
            this.baseService = baseService;
        }

        public async Task NapClientSyncDataAsync(int? userId)
        {
            var data = await GetDataFromNapClient(userId).ConfigureAwait(false);
            await InsertNapClientData(data).ConfigureAwait(false);
        }

        public async Task OicSyncDataAsync()
        {
            var data = await GetDataFromOIC().ConfigureAwait(false);
            await InsertOICData(data).ConfigureAwait(false);
        }

        private async Task<NapDataCrawler> GetDataFromNapClient(int? userId)
        {
            var criteria = new GetDataFromNapClientCriteria
            {
                SiteId = appSettings.Value.SiteId,
                UserId = userId
            };

            return await crawlDataRepository.FetchDataFromNapClient(criteria).ConfigureAwait(false);
        }

        private async Task InsertNapClientData(NapDataCrawler data)
        {
            var command = new InsertNapClientDataCommand
            {
                Functions = JsonConvert.SerializeObject(data.NAP_Functions),
                NapGroups = JsonConvert.SerializeObject(data.NAP_Groups),
                GroupFunction = JsonConvert.SerializeObject(data.NAP_GroupFunction),
                MenuItems = JsonConvert.SerializeObject(data.NAP_MenuItems),
                OnUser = JsonConvert.SerializeObject(data.NAP_OnUser),
                Sites = JsonConvert.SerializeObject(data.NAP_Sites),
                SiteURL = JsonConvert.SerializeObject(data.NAP_SiteURL),
                UserFunction = JsonConvert.SerializeObject(data.NAP_UserFunction),
                Users = JsonConvert.SerializeObject(data.NAP_Users),
                UserSite = JsonConvert.SerializeObject(data.NAP_UserSite)
            };

            await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private async Task<OicDataCrawler> GetDataFromOIC()
        {
            var criteria = new GetDataFromOICCriteria();

            return await crawlDataRepository.FetchDataFromOIC(criteria).ConfigureAwait(false);
        }

        private async Task InsertOICData(OicDataCrawler data)
        {
            var command = new InsertOICDataCommand
            {
                Licensees = JsonConvert.SerializeObject(data.Licensees),
                LicenseeStatuses = JsonConvert.SerializeObject(data.LicenseeStatuses),
                LicenseeProducts = JsonConvert.SerializeObject(data.LicenseeProducts),
                LicenseeTypes = JsonConvert.SerializeObject(data.LicenseeTypes),
                Products = JsonConvert.SerializeObject(data.Products),
            };

            await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }
    }
}