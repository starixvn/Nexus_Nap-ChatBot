namespace Fanex.NCB.Criteria.SettingConfiguration;

using Data.Repository;

public class GetNotificationTypeDataCriteria : CriteriaBase
{
    public int NotificationTypeId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_NotificationTypeData_Sel";
    }

    public override bool IsValid()
    {
        return NotificationTypeId > 0;
    }
}