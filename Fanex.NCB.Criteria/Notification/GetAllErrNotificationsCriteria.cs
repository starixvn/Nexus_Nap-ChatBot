namespace Fanex.NCB.Criteria.Notification;

using Data.Repository;

public class GetAllErrNotificationsCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_NotificationList_ErrorNotifications_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}