namespace Fanex.NCB.Main.ViewModel.Notification
{
    using System;

    public class CreateNewNotificationViewModel
    {
        public string ChannelIds { get; set; }

        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string RecipientIds { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime SendDate { get; set; }

        public bool SendImmediately { get; set; }

        public string Platforms { get; set; }

        public string IntegrationMethodIds { get; set; }
    }
}