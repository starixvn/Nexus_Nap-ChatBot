namespace Fanex.NCB.Criteria.NotificationTemplate;

using Fanex.NCB.Shared.Models;

public class DeleteNotificationTemplateCommand : BaseCommand
{
    public int NotificationTemplateId { get; set; }

    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationTemplate_Del";
    }

    public override bool IsValid()
    {
        return NotificationTemplateId > 0 && UserId > 0;
    }
}