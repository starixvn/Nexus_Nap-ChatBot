namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.Data.Repository;

    public class GetNotificationTypeDataCriteria : CriteriaBase
    {
        public int NotificationTypeId { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_NotificationTypeData_Sel";

        public override bool IsValid() => NotificationTypeId > 0;
    }
}