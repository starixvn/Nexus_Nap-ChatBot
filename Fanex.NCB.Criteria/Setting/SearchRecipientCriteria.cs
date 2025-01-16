namespace Fanex.NCB.Criteria.Setting
{
    using Data.Repository;

    public class SearchRecipientCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

        public override string GetSettingKey() => "NCB_Setting_SearchRecipient_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Keyword);
    }
}