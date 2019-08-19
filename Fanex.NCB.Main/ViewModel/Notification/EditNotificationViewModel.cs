namespace Fanex.NCB.Main.ViewModel.Notification
{
    using System;

    public class EditNotificationViewModel
    {
        public int NotificationId { get; set; }

        public string RecipientIds { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime SendDate { get; set; }

        public bool SendImmediately { get; set; }
    }
}