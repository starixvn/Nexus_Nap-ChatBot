namespace Fanex.NCB.Criteria.Notification;

using Fanex.NCB.Shared.Models;

public class ResolveNotificationCommand : BaseCommand
{
    public int NotificationId { get; set; }

    public string ResolvedRemark { get; set; }

    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationList_Resolve_Upd";
    }

    public override bool IsValid()
    {
        return NotificationId > 0 && !string.IsNullOrWhiteSpace(ResolvedRemark) && UserId > 0;
    }
}