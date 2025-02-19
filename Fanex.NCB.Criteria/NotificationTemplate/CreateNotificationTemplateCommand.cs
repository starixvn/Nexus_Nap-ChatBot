namespace Fanex.NCB.Criteria.NotificationTemplate;

using Fanex.NCB.Shared.Models;

public class CreateNotificationTemplateCommand : BaseCommand
{
    public int NotificationTypeId { get; set; }

    public string NotificationTemplateName { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public int CreatorId { get; set; }

    public int NotificationTemplateId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationTemplate_Ins";
    }

#pragma warning disable S1067 // Expressions should not be too complex

    public override bool IsValid()
    {
        return NotificationTypeId > 0 && CreatorId > 0 && !string.IsNullOrWhiteSpace(NotificationTemplateName)
               && !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content);
    }

#pragma warning restore S1067 // Expressions should not be too complex
}