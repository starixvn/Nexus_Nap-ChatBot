namespace Fanex.NCB.Criteria.LocalNap.Site
{
    using Fanex.Data.Repository;

    public class GetByIdSiteCriteria : CriteriaBase
    {
        public int SiteId { get; set; }

        public override string GetSettingKey() => "NAP_Site_GetById";

        public override bool IsValid() => SiteId > 0;
    }
}
