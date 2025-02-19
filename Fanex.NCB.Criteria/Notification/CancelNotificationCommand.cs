namespace Fanex.NCB.Criteria.Notification;

using Fanex.NCB.Shared.Models;

public class CancelNotificationCommand : BaseCommand
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationList_Cancel_Upd";
    }

    public override bool IsValid()
    {
        return NotificationId > 0 && UserId > 0;
    }
}