using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Bot.Models.DTO
{
    public class BotConversation
    {

        public int GroupID { get; set; }
        public string ConversationID { get; set; }
        public string Name { get; set; }
        public bool IsBotActive { get; set; }
        public string ChannelID { get; set; }
        public string BotName { get; set; }
        public DateTime LastUpdate { get; set; }

        public string ServiceUrl { get; set; }
    }
}
