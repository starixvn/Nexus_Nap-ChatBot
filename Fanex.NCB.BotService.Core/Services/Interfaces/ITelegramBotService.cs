namespace Fanex.NCB.BotService.Core.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public interface ITelegramBotService
    {
        Task<User> GetBotInfoAsync(int botId);

        Task<Message> SendTextMessageAsync(int botId, long chatId, string text, ParseMode parseMode = ParseMode.MarkdownV2);
    }
}