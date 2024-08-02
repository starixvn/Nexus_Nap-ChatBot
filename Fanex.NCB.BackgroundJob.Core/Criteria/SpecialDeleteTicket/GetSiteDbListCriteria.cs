namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetSiteDbListCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Acc_SiteDBConfigSel";

        public override bool IsValid() => true;
    }
}