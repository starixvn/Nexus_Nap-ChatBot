namespace Fanex.NCB.BotService.Core.Models
{
    using System.Net.Http;
    using Telegram.Bot;

    public class TelegramBotConfigurations
    {
        public long BotId { get; set; }
        public string Token { get; set; }
        public HttpClient HttpClient { get; set; } = new HttpClient();
        public ITelegramBotClient BotClient => new TelegramBotClient(Token, HttpClient);
    }
}