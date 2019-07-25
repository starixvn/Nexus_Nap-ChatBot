namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class GetChatGroupTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_ChatGroupType_Sel";

        public override bool IsValid() => true;
    }
}