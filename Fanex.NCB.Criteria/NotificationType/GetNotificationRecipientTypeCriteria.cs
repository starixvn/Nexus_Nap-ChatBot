namespace Fanex.NCB.Criteria.NotificationType
{
    using Fanex.Data.Repository;

    public class GetNotificationRecipientTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_RecipientType_Sel";

        public override bool IsValid() => true;
    }
}