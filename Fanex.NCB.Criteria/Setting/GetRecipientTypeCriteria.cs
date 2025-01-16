namespace Fanex.NCB.Criteria.Setting
{
    using Data.Repository;

    public class GetRecipientTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Common_RecipientType_Sel";

        public override bool IsValid() => true;
    }
}