namespace Fanex.NCB.Core.Models
{
    public class MonitorChatbot
    {
        public bool IsMonitoring { get; set; }

        public string BotName { get; set; }

        public string BotConversationId { get; set; }

        public string ChannelId { get; set; }

        public string ServiceUrl { get; set; }

        public string ConversationId { get; set; }

        public string Message { get; set; }
    }
}