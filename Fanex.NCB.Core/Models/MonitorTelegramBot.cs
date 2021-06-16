namespace Fanex.NCB.Core.Models
{
    public class MonitorTelegramBot
    {
        public long BotId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }
        public bool IsMonitoring { get; set; }
    }
}