namespace Fanex.NCB.BotService.Core.Services
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface ITelegramBotService
    {
        Task SetWebhookAsync();

        Task<User> GetBotInfoAsync(long botId);

        Task SendTextMessageAsync(long botId, long chatId, string messageContent);
    }
}