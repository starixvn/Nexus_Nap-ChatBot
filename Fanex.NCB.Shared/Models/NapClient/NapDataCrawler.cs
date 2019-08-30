namespace Fanex.NCB.Shared.Models.NapClient
{
    using System.Collections.Generic;

    public class NapDataCrawler
    {
        public IEnumerable<NapFunction> NAP_Functions { get; set; }

        public IEnumerable<NapGroup> NAP_Groups { get; set; }

        public IEnumerable<NapGroupFunction> NAP_GroupFunction { get; set; }

        public IEnumerable<NapMenuItem> NAP_MenuItems { get; set; }

        public IEnumerable<NapOnUser> NAP_OnUser { get; set; }

        public IEnumerable<NapSite> NAP_Sites { get; set; }

        public IEnumerable<NapSiteURL> NAP_SiteURL { get; set; }

        public IEnumerable<NapUserFunction> NAP_UserFunction { get; set; }

        public IEnumerable<NapUser> NAP_Users { get; set; }

        public IEnumerable<NapUserSite> NAP_UserSite { get; set; }
    }
}