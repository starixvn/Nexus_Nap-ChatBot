using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NCB.AutoJobs.Core.Models.IntegrationMethod;
using Fanex.NCB.AutoJobs.Core.Models.NapUserControl;
using Fanex.NCB.AutoJobs.Core.Models.OIC;

namespace Fanex.NCB.AutoJobs.Core.Repository
{
    public interface ISyncDataRepository
    {
        Task<NapData> FetchDataFromNapUserControl(ICriteria criteria);

        Task<OicData> FetchDataFromOIC(ICriteria criteria);
    }

    public class SyncDataRepository : ISyncDataRepository
    {
        public async Task<NapData> FetchDataFromNapUserControl(ICriteria criteria)
        {
            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var multi = await db.QueryMultipleAsync(criteria);
            var data = new NapData
            {
                NapFunctions = multi.Read<NapFunction>(),
                NapGroups = multi.Read<NapGroup>(),
                NapGroupFunction = multi.Read<NapGroupFunction>(),
                NapMenuItems = multi.Read<NapMenuItem>(),
                NapOnUser = multi.Read<NapOnUser>(),
                NapSites = multi.Read<NapSite>(),
                NapSiteURL = multi.Read<NapSiteURL>(),
                NapUserFunction = multi.Read<NapUserFunction>(),
                NapUsers = multi.Read<NapUser>(),
                NapUserSite = multi.Read<NapUserSite>(),
                Customer = multi.Read<Customer>(),
                CustInfo = multi.Read<CustInfo>()
            };

            return data;
        }

        public async Task<OicData> FetchDataFromOIC(ICriteria criteria)
        {
            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var multi = await db.QueryMultipleAsync(criteria);
            var data = new OicData
            {
                Licensees = multi.Read<Licensee>(),
                LicenseeStatuses = multi.Read<LicenseeStatus>(),
                LicenseeProducts = multi.Read<LicenseeProduct>(),
                LicenseeTypes = multi.Read<LicenseeType>(),
                Products = multi.Read<Product>(),
                Platforms = multi.Read<Platform>(),
                IntegrationMethod = multi.Read<IntegrationMethodInfo>(),
                LicenseeIntegrationMethod = multi.Read<LicenseeIntegrationMethod>()
            };

            return data;
        }
    }
}