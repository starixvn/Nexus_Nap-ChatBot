using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping
{
    public class ChatGroupMappingItem
    {
        public string BotName { get; set; }
        public string ConversationID { get; set; }
        public string GroupName { get; set; }
        public int RecipientTypeID { get; set; }
        public string RecipientTypeName { get; set; }
        public int RecipientID { get; set; }
        public string RecipientName { get; set; }
        public string LicenseeStatus { get; set; }
        public int MappingStatusID { get; set; }
        public string MappingStatusName { get; set; }
        public string Remark { get; set; }     
    }
}
