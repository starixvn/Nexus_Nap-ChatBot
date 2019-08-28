namespace Fanex.NCB.Criteria.CrawlData
{
    using Fanex.Data.Repository;

    public class GetDataFromNapClientCriteria : CriteriaBase
    {
        public int SiteId { get; set; }

        public override string GetSettingKey() => "NapClient_GetData";

        public override bool IsValid() => SiteId > 0;
    }
}