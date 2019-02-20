using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Bot.Models.DTO
{
    public class BotConversationMember
    {
        public string ConversationID { get; set; }
        public string MemberId { get; set; }
        public string Name { get; set; }
        public bool IsBot { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
