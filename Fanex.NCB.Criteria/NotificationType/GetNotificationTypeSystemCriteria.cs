namespace Fanex.NCB.Criteria.NotificationType;

using Data.Repository;

public class GetNotificationTypeSystemCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Setting_System_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}