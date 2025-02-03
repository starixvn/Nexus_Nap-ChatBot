namespace Fanex.NCB.Criteria.Notification;

using Data.Repository;

public class GetEditNotificationInfoCriteria : CriteriaBase
{
    public int InNotificationId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_EditNotification_Sel";
    }

    public override bool IsValid()
    {
        return InNotificationId > 0;
    }
}