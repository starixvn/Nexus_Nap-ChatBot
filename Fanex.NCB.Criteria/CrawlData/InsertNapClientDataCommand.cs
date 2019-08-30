namespace Fanex.NCB.Criteria.CrawlData
{
    using Fanex.NCB.Shared.Models;

    public class InsertNapClientDataCommand : BaseCommand
    {
        public string Functions { get; set; }

        public string NapGroups { get; set; }

        public string GroupFunction { get; set; }

        public string MenuItems { get; set; }

        public string OnUser { get; set; }

        public string Sites { get; set; }

        public string SiteURL { get; set; }

        public string UserFunction { get; set; }

        public string Users { get; set; }

        public string UserSite { get; set; }

        public override string GetSettingKey() => "NAP_CrawlData_InsUpdDataNapUserControl";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Functions);
    }
}