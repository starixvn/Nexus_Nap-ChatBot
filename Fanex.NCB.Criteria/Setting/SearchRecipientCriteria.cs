namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class SearchRecipientCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public override string GetSettingKey() => "NCB_Setting_SearchRecipient_Sel";

        public override bool IsValid() => true;
    }
}