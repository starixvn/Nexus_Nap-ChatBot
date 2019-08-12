namespace Fanex.NCB.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetRecipientsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Common_Recipient_Sel";

        public override bool IsValid() => true;
    }
}