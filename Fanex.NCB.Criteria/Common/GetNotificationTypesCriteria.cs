namespace Fanex.NCB.Criteria.Common;

using Data.Repository;

public class GetNotificationTypesCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Common_NotificationType_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}