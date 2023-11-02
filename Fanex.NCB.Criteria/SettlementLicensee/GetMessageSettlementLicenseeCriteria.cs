namespace Fanex.NCB.Criteria.SettlementLicensee
{
    using Fanex.Data.Repository;

    public class GetMessageSettlementLicenseeCriteria : CriteriaBase
    {
        public int BatchSize { get; set; }

        public int LastMsgTargetId { get; set; }

        public string SiteAsJSON { get; set; }

        public string DepCustSAsJSON { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_Settlement_Licensee_MessageSel";

        public override bool IsValid() => true;
    }
}
