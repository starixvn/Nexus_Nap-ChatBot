namespace Fanex.NCB.Criteria.Notification;

using Data;
using Data.Repository;

public class GetUpdatedNotificationCriteria : CriteriaBase
{
    [SpParam(AllowNull = true)] public string NotificationIds { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationList_UpdatedNotification_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}