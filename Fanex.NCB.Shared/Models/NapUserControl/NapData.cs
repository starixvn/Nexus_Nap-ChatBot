namespace Fanex.NCB.Shared.Models.NapUserControl;

using System.Collections.Generic;

public class NapData
{
    public IEnumerable<NapFunction> NapFunctions { get; set; }

    public IEnumerable<NapGroup> NapGroups { get; set; }

    public IEnumerable<NapGroupFunction> NapGroupFunction { get; set; }

    public IEnumerable<NapMenuItem> NapMenuItems { get; set; }

    public IEnumerable<NapOnUser> NapOnUser { get; set; }

    public IEnumerable<NapSite> NapSites { get; set; }

    public IEnumerable<NapSiteURL> NapSiteURL { get; set; }

    public IEnumerable<NapUserFunction> NapUserFunction { get; set; }

    public IEnumerable<NapUser> NapUsers { get; set; }

    public IEnumerable<NapUserSite> NapUserSite { get; set; }

    public IEnumerable<Customer> Customer { get; set; }

    public IEnumerable<CustInfo> CustInfo { get; set; }
}