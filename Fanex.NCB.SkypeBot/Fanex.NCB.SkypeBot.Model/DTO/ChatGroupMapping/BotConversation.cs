using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping
{
    public class BotConversation
    {
        public string GroupName { get; set; }
        public int GroupID { get; set; }
        public string ConversationID { get; set; }
        public string Remark { get; set; }
        public int RecipientTypeId { get; set; }
        public string RecipientTypeName { get; set; }
        public bool IsMapped { get; set; }
    }
}
