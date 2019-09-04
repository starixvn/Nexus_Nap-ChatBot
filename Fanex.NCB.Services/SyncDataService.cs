namespace Fanex.NCB.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SyncData;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
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
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SyncDataService(IOptionsSnapshot<AppSettings> appSettings
            , ISyncDataRepository crawlDataRepository,
            IBaseService baseService)
        {
            this.appSettings = appSettings;
            this.crawlDataRepository = crawlDataRepository;
            this.baseService = baseService;
        }

        public async Task NapUserControlSyncDataAsync()
        {
            var data = await GetDataFromNapUserControl(null).ConfigureAwait(false);
            await InsertNapData(data).ConfigureAwait(false);
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

        private async Task InsertNapData(NapData data)
        {
            var command = new InsertNapUserControlDataCommand
            {
                Functions = ConvertToJsonWithDateTimeFormatString(data.NapFunctions),
                NapGroups = ConvertToJsonWithDateTimeFormatString(data.NapGroups),
                GroupFunction = ConvertToJsonWithDateTimeFormatString(data.NapGroupFunction),
                MenuItems = ConvertToJsonWithDateTimeFormatString(data.NapMenuItems),
                OnUser = ConvertToJsonWithDateTimeFormatString(data.NapOnUser),
                Sites = ConvertToJsonWithDateTimeFormatString(data.NapSites),
                SiteURL = ConvertToJsonWithDateTimeFormatString(data.NapSiteURL),
                UserFunction = ConvertToJsonWithDateTimeFormatString(data.NapUserFunction),
                Users = ConvertToJsonWithDateTimeFormatString(data.NapUsers),
                UserSite = ConvertToJsonWithDateTimeFormatString(data.NapUserSite)
            };

            await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private async Task<CommandResult> InsertNapDataOnUser(NapData data, int userId)
        {
            var command = new UpdateNapDataByUserIdCommand
            {
                Functions = ConvertToJsonWithDateTimeFormatString(data.NapFunctions),
                NapGroups = ConvertToJsonWithDateTimeFormatString(data.NapGroups),
                GroupFunction = ConvertToJsonWithDateTimeFormatString(data.NapGroupFunction),
                MenuItems = ConvertToJsonWithDateTimeFormatString(data.NapMenuItems),
                OnUser = ConvertToJsonWithDateTimeFormatString(data.NapOnUser),
                Sites = ConvertToJsonWithDateTimeFormatString(data.NapSites),
                SiteURL = ConvertToJsonWithDateTimeFormatString(data.NapSiteURL),
                UserFunction = ConvertToJsonWithDateTimeFormatString(data.NapUserFunction),
                Users = ConvertToJsonWithDateTimeFormatString(data.NapUsers),
                UserSite = ConvertToJsonWithDateTimeFormatString(data.NapUserSite),
                UserId = userId
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
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
                Licensees = ConvertToJsonWithDateTimeFormatString(data.Licensees),
                LicenseeStatuses = ConvertToJsonWithDateTimeFormatString(data.LicenseeStatuses),
                LicenseeProducts = ConvertToJsonWithDateTimeFormatString(data.LicenseeProducts),
                LicenseeTypes = ConvertToJsonWithDateTimeFormatString(data.LicenseeTypes),
                Products = ConvertToJsonWithDateTimeFormatString(data.Products),
            };

            await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private string ConvertToJsonWithDateTimeFormatString(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { DateFormatString = DateTimeFormat });
        }

        public async Task<CommandResult> NapSSOSyncDataByUserIdAsync(int userId)
        {
            var data = await GetDataFromNapUserControl(userId).ConfigureAwait(false);
            return await InsertNapDataOnUser(data, userId).ConfigureAwait(false);
        }
    }
}