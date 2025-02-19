namespace Fanex.NCB.Criteria.NotificationType;

using Fanex.NCB.Shared.Models;

public class DeleteNotificationTypeCommand : BaseCommand
{
    public int NotificationTypeId { get; set; }

    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationType_Del";
    }

    public override bool IsValid()
    {
        return NotificationTypeId > 0;
    }
}