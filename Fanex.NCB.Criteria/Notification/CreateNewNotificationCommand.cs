namespace Fanex.NCB.Criteria.Notification
{
    using System;
    using Fanex.NCB.Shared.Models;

    public class CreateNewNotificationCommand : BaseCommand
    {
        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string RecipientIDLst { get; set; }

        public string Title { get; set; }

        public string ContentLst { get; set; }

        public DateTime ConfigSendTime { get; set; }

        public bool SendImmediately { get; set; }

        public int CreatorId { get; set; }

        public string NotificationIDLst { get; set; }

        public string Platforms { get; set; }

        public override string GetSettingKey() => "NCB_NewNotification_Ins";

        public override bool IsValid()
        {
            bool isValidNotiInfo = NotificationTypeId > 0 && CreatorId > 0;
            return isValidNotiInfo && !string.IsNullOrWhiteSpace(RecipientIDLst) && !string.IsNullOrWhiteSpace(Title)
                            && !string.IsNullOrWhiteSpace(ContentLst);
        }
    }
}