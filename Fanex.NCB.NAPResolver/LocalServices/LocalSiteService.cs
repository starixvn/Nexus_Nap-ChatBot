namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Criteria.LocalNap.Site;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class LocalSiteService : ISiteService
    {
        private readonly IBaseService baseService;

        public LocalSiteService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

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

        public async Task<bool> IsActivatedAsync(int siteId)
        {
            var criteria = new GetByIdSiteCriteria
            {
                SiteId = siteId
            };

            bool isActive = false;
            var site = await baseService.GetDataEntityAsync<Site>(criteria);

            if (site != null)
            {
                isActive = site.IsActive;
            }

            return isActive;
        }
    }
}