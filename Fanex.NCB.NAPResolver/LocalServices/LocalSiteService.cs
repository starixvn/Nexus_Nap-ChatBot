namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;

    public class LocalSiteService : ISiteService
    {
        public IEnumerable<MenuItem> GetMenuItemBySiteId(int siteId, string enviroment, bool? isActive = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<MenuItem>> GetMenuItemBySiteIdAsync(int siteId, string enviroment, bool? isActive = null)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SiteUrl> GetUrlById(int siteId, string environment)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<SiteUrl>> GetUrlByIdAsync(int siteId, string environment)
        {
            throw new System.NotImplementedException();
        }

        public bool IsActivated(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsActivatedAsync(int siteId)
        {
            throw new System.NotImplementedException();
        }
    }
}