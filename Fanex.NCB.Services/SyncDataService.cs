namespace Fanex.NCB.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.CrawlData;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models.NapUserControl;
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

        public async Task NapUserControlSyncDataAsync(int? userId)
        {
            var data = await GetDataFromNapUserControl(userId).ConfigureAwait(false);
            await InsertNapUserControlData(data).ConfigureAwait(false);
        }

        public async Task OicSyncDataAsync()
        {
            var data = await GetDataFromOIC().ConfigureAwait(false);
            await InsertOICData(data).ConfigureAwait(false);
        }

        private async Task<NapData> GetDataFromNapUserControl(int? userId)
        {
            var criteria = new GetDataFromNapUserControlCriteria
            {
                SiteId = appSettings.Value.SiteId,
                UserId = userId
            };

            return await crawlDataRepository.FetchDataFromNapUserControl(criteria).ConfigureAwait(false);
        }

        private async Task InsertNapUserControlData(NapData data)
        {
            var command = new InsertNapUserControlDataCommand
            {
                Functions = JsonConvert.SerializeObject(data.NapFunctions),
                NapGroups = JsonConvert.SerializeObject(data.NapGroups),
                GroupFunction = JsonConvert.SerializeObject(data.NapGroupFunction),
                MenuItems = JsonConvert.SerializeObject(data.NapMenuItems),
                OnUser = JsonConvert.SerializeObject(data.NapOnUser),
                Sites = JsonConvert.SerializeObject(data.NapSites),
                SiteURL = JsonConvert.SerializeObject(data.NapSiteURL),
                UserFunction = JsonConvert.SerializeObject(data.NapUserFunction),
                Users = JsonConvert.SerializeObject(data.NapUsers),
                UserSite = JsonConvert.SerializeObject(data.NapUserSite)
            };

            await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private async Task<OicData> GetDataFromOIC()
        {
            var criteria = new GetDataFromOICCriteria();

            return await crawlDataRepository.FetchDataFromOIC(criteria).ConfigureAwait(false);
        }

        private async Task InsertOICData(OicData data)
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