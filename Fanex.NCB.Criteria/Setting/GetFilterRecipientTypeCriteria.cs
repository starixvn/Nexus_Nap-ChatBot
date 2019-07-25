namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class GetFilterRecipientTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_RecipientType_Sel";

        public override bool IsValid() => true;
    }
}