namespace Fanex.NCB.BotService.Core.Models
{
    using System.Net.Http;
    using Telegram.Bot;

    public class TelegramBotConfigurations
    {
        public int BotId { get; set; }
        public string Token { get; set; }
        public HttpClient HttpClient { get; set; } = new HttpClient();
        public TelegramBotClient BotClient => new TelegramBotClient(Token, HttpClient);
    }
}