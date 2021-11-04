namespace Fanex.NCB.BotService.Core.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public interface ITelegramBotService
    {
        Task SetWebhookAsync();

        Task<User> GetBotInfoAsync(long botId);

        Task<Message> SendTextMessageAsync(string transid, long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2);
    }
}