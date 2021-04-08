namespace Fanex.NCB.BotService.Core.Models
{
    using Telegram.Bot;

    public class TelegramBotConfigurations
    {
        public int BotId { get; set; }
        public string Token { get; set; }

        public TelegramBotClient BotClient => new TelegramBotClient(Token);
    }
}