namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class SearchChatGroupNameCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

        public override string GetSettingKey() => "NCB_Setting_SearchChatGroup_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Keyword);
    }
}