namespace Fanex.NCB.Criteria.NotificationType
{
    using Fanex.NCB.Shared.Models;

    public class UpdateNotificationTypeCommand : BaseCommand
    {
        public int NotificationTypeId { get; set; }

        public string NotificationTypeName { get; set; }

        public string RecipientTypeIds { get; set; }

        public string ChatGroupTypeIds { get; set; }

        public int SystemId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Upd";

        public override bool IsValid()
        {
            if(NotificationTypeId <= 0)
            {
                return false;
            }

            return SystemId > 0 &&
                    !string.IsNullOrWhiteSpace(NotificationTypeName) &&
                    !string.IsNullOrWhiteSpace(RecipientTypeIds) &&
                    !string.IsNullOrWhiteSpace(ChatGroupTypeIds);
        }        
    }
}