namespace Fanex.NCB.Repository
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Shared.Models.NapClient;

    public class CrawlDataRepository : ICrawlDataRepository
    {
        public async Task<NapDataCrawler> FetchDataFromNapClient(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var data = new NapDataCrawler
                    {
                        NAP_Functions = multi.Read<NapFunction>(),
                        NAP_Groups = multi.Read<NapGroup>(),
                        NAP_GroupFunction = multi.Read<NapGroupFunction>(),
                        NAP_MenuItems = multi.Read<NapMenuItem>(),
                        NAP_OnUser = multi.Read<NapOnUser>(),
                        NAP_Sites = multi.Read<NapSite>(),
                        NAP_SiteURL = multi.Read<NapSiteURL>(),
                        NAP_UserFunction = multi.Read<NapUserFunction>(),
                        NAP_UserMarkedMenus = multi.Read<NapUserMarkedMenu>(),
                        NAP_Users = multi.Read<NapUser>(),
                        NAP_UserSite = multi.Read<NapUserSite>()
                    };

                    return data;
                }
            }
        }
    }
}