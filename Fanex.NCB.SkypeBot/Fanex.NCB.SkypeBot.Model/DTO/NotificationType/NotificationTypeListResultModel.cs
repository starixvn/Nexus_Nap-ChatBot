using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.DTO.NotificationType
{
    public class NotificationTypeListResultModel
    {
        public List<NotificationType> Data { get; set; }
        public int Count { get; set; }
    }
}
