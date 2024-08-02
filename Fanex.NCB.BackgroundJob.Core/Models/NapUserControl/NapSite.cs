namespace Fanex.NCB.AutoJobs.Core.Models.NapUserControl
{
    public class NapSite
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string Description { get; set; }

        public string URL { get; set; }

        public string TestURL { get; set; }

        public bool IsActive { get; set; }

        public string Environment { get; set; }

        public string SecretAccessKey { get; set; }

        public string SiteKey { get; set; }
    }
}