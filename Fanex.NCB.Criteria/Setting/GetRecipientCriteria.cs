namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class GetRecipientCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_Recipient_Sel";

        public override bool IsValid() => true;
    }
}