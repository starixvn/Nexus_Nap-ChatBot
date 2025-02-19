namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl;

public class NapSiteURL
{
    public int SiteURLId { get; set; }

    public int SiteId { get; set; }

    public string URL { get; set; }

    public string Environment { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }
}