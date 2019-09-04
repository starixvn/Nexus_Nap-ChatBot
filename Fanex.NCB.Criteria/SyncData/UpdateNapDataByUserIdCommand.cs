namespace Fanex.NCB.Criteria.SyncData
{
    using Fanex.NCB.Shared.Models;

    public class UpdateNapDataByUserIdCommand : BaseCommand
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

        public int UserId { get; set; }

        public override string GetSettingKey() => "NAP_UserControl_SyncData_ByUserId";

        public override bool IsValid() => UserId > 0;
    }
}