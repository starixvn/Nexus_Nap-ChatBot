namespace Fanex.NCB.Criteria.NotificationTemplate
{
    using Fanex.NCB.Shared.Models;

    public class EditNotificationTemplateCommand : BaseCommand
    {
        public int NotificationTemplateId { get; set; }

        public int NotificationTypeId { get; set; }

        public string NotificationTemplateName { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int ModifierId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationTemplate_Upd";

        private bool isValidIntParam() => NotificationTemplateId > 0 && NotificationTypeId > 0 && ModifierId > 0;

        public override bool IsValid() => isValidIntParam() &&
            !string.IsNullOrWhiteSpace(NotificationTemplateName)
            && !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content);
    }
}