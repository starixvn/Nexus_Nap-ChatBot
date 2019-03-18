using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping
{
    public class BotConversationMember
    {
        public string BotName { get; set; }
        public string ConversationID { get; set; }
        public bool IsActive { get; set; }
    }
}
