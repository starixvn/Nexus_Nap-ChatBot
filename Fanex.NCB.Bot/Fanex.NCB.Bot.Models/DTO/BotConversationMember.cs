using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Bot.Models.DTO
{
    public class BotConversationMember
    {
        public int ID { get; set; }
        public string ConversationID { get; set; }
        public string MemberId { get; set; }
        public string Name { get; set; }
        public bool IsBot { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
    }
}
