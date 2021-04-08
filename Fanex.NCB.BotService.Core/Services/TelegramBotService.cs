namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Models;
    using Microsoft.Extensions.Options;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly IEnumerable<TelegramBotConfigurations> botConfigurations;

        public TelegramBotService(IOptions<List<TelegramBotConfigurations>> botConfigurations)
        {
            this.botConfigurations = botConfigurations.Value;
        }

        public async Task<User> GetBotInfoAsync(int botId)
        {
            TelegramBotConfigurations bot = botConfigurations.FirstOrDefault(x => x.BotId == botId);
            return await bot.BotClient.GetMeAsync();
        }

        public async Task<Message> SendTextMessageAsync(int botId, long chatId, string text, ParseMode parseMode = ParseMode.MarkdownV2)
        {
            TelegramBotConfigurations bot = botConfigurations.FirstOrDefault(x => x.BotId == botId);
            return await bot.BotClient.SendTextMessageAsync(new ChatId(chatId), text, parseMode);
        }
    }
}