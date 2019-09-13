namespace Fanex.NCB.Criteria.NotificationType
{
    using Fanex.NCB.Shared.Models;

    public class InsertNotificationTypeCommand : BaseCommand
    {
        public string NotificationTypeName { get; set; }

        public string RecipientTypeIds { get; set; }

        public string ChatGroupTypeIds { get; set; }

        public int SystemId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Ins";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(NotificationTypeName) &&
                                            SystemId > 0 && 
                                            !string.IsNullOrWhiteSpace(RecipientTypeIds) &&
                                            !string.IsNullOrWhiteSpace(ChatGroupTypeIds);
    }
}