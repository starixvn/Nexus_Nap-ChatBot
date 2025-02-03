namespace Fanex.NCB.Criteria.SendNotification;

using Data.Repository;

public class GetNotificationsToSendCriteria : CriteriaBase
{
    public string UpdatedNotificationIds { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SendNotification_GetPlanRetryNotifUpdStatus";
    }

    public override bool IsValid()
    {
        return true;
    }
}