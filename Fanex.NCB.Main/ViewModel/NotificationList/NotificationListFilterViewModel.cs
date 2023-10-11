namespace Fanex.NCB.Main.ViewModel.NotificationList
{
    using System.Collections.Generic;
    using Fanex.NCB.Models.Common;

    public class NotificationListFilterViewModel
    {
        public string NotificationTypeIds { get; set; }
        public string NotificationStatusIds { get; set; }
        public string RecipientIds { get; set; }
        public string ChannelIds { get; set; }
        public string ProductIds { get; set; }
        public IEnumerable<MultiSelectViewModel> NotificationTypes { get; set; }
        public IEnumerable<User> Senders { get; set; }
        public IEnumerable<MultiSelectViewModel> NotificationStatuses { get; set; }
        public IEnumerable<MultiSelectViewModel> Recipients { get; set; }
        public IEnumerable<MultiSelectViewModel> Channels { get; set; }
        public IEnumerable<MultiSelectViewModel> Products { get; set; }
    }
}