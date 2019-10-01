namespace Fanex.NCB.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public abstract class NapUserControlSyncDataBaseCommand : NonQueryCommand
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

        public string Customer { get; set; }

        public string CustInfo { get; set; }
    }
}