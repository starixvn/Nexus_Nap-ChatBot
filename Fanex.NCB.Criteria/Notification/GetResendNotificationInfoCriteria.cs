namespace Fanex.NCB.Criteria.Notification;

using Data.Repository;

public class GetResendNotificationInfoCriteria : CriteriaBase
{
    public int InNotificationId { get; set; }

    public bool InIsSendAll { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ResendNotification_Sel";
    }

    public override bool IsValid()
    {
        return InNotificationId > 0;
    }
}