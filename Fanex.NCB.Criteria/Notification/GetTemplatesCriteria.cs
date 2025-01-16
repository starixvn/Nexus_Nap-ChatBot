namespace Fanex.NCB.Criteria.Notification
{
    using Data.Repository;

    public class GetTemplatesCriteria : CriteriaBase
    {
        public int NotificationTypeId { get; set; }

        public override string GetSettingKey() => "NCB_NewNotification_GetTemplates";

        public override bool IsValid() => NotificationTypeId > 0;
    }
}