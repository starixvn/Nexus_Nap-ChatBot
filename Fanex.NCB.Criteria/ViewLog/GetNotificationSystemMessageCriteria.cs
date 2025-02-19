namespace Fanex.NCB.Criteria.ViewLog;

using Data;
using Data.Repository;

public class GetNotificationSystemMessageCriteria : CriteriaBase
{
    [SpParam(AllowNull = true)] public int? NotificationId { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ViewLog_NotificationSystemMessage_Sel";
    }

    public override bool IsValid()
    {
        return PageIndex > 0 && PageSize > 0;
    }
}