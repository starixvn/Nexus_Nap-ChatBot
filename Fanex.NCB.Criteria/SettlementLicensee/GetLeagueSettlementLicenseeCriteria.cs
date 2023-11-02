namespace Fanex.NCB.Criteria.SettlementLicensee
{
    using Fanex.Data.Repository;

    public class GetLeagueSettlementLicenseeCriteria : CriteriaBase
    {
        public string Message { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_Settlement_Licensee_LeagueSel";

        public override bool IsValid() => true;
    }
}
