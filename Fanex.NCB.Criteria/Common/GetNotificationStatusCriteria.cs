namespace Fanex.NCB.Criteria.Common;

using Data.Repository;

public class GetNotificationStatusCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Common_NotificationStatus_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}