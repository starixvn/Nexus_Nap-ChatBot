namespace Fanex.NCB.Services
{
    using Logging;
    using Fanex.NCB.Core.Models;
    using Criteria.SyncData;
    using Fanex.NCB.Repository.Interfaces;
    using Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.NapUserControl;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public class SyncDataService : ISyncDataService
    {
        private readonly IOptions<AppSettings> appSettings;
        private readonly ISyncDataRepository syncDataRepository;
        private readonly IBaseService baseService;
        private readonly ILogger logger;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SyncDataService(IOptions<AppSettings> appSettings,
            ISyncDataRepository syncDataRepository,
            IBaseService baseService,
            ILogger logger)
        {
            this.appSettings = appSettings;
            this.syncDataRepository = syncDataRepository;
            this.baseService = baseService;
            this.logger = logger;
        }

        public async Task SyncNapDataForSSOLogin(int userId)
        {
            try
            {
                await NapSSOSyncDataByUserIdAsync(userId);
            }
            catch (Exception exception)
            {
                string errMsg = string.Format(
                CultureInfo.InvariantCulture,
                "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                exception.Message,
                exception.Source,
                string.Empty,
                exception.TargetSite,
                exception.StackTrace);
                await logger.ErrorAsync(errMsg);
            }
        }

        private async Task<NapData> GetDataFromNapUserControl(int? userId)
        {
            var criteria = new GetDataFromNapUserControlCriteria
            {
                SiteId = appSettings.Value.SiteId,
                UserId = userId
            };

            return await syncDataRepository.FetchDataFromNapUserControl(criteria);
        }

        private async Task<CommandResult> InsertNapDataOnUser(NapData data, int userId)
        {
            var command = new UpdateNapDataByUserIdCommand
            {
                Functions = ConvertToJsonWithDateTimeFormatString(data.NapFunctions),
                UserSite = ConvertToJsonWithDateTimeFormatString(data.NapUserSite),
                Customer = ConvertToJsonWithDateTimeFormatString(data.Customer),
                CustInfo = ConvertToJsonWithDateTimeFormatString(data.CustInfo),
                NapGroups = ConvertToJsonWithDateTimeFormatString(data.NapGroups),
                GroupFunction = ConvertToJsonWithDateTimeFormatString(data.NapGroupFunction),
                MenuItems = ConvertToJsonWithDateTimeFormatString(data.NapMenuItems),
                OnUser = ConvertToJsonWithDateTimeFormatString(data.NapOnUser),
                Sites = ConvertToJsonWithDateTimeFormatString(data.NapSites),
                SiteURL = ConvertToJsonWithDateTimeFormatString(data.NapSiteURL),
                UserFunction = ConvertToJsonWithDateTimeFormatString(data.NapUserFunction),
                Users = ConvertToJsonWithDateTimeFormatString(data.NapUsers),
                UserId = userId
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        private static string ConvertToJsonWithDateTimeFormatString(object data)
            => JsonConvert.SerializeObject(data, new JsonSerializerSettings { DateFormatString = DateTimeFormat });

        private async Task NapSSOSyncDataByUserIdAsync(int userId)
        {
            var data = await GetDataFromNapUserControl(userId);
            //await InsertNapDataOnUser(data, userId);
        }
    }
}