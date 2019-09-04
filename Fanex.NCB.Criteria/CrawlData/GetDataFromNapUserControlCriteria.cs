namespace Fanex.NCB.Criteria.CrawlData
{
    using Fanex.Data.Repository;

    public class GetDataFromNapUserControlCriteria : CriteriaBase
    {
        public int SiteId { get; set; }

        public int? UserId { get; set; }

        public override string GetSettingKey() => "NAP_UserControl_GetDataBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}