namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly AppSettings appSettings;
        private readonly IEnumerable<TelegramBotConfigurations> botConfigurations;
        private readonly ILogger<TelegramBotService> filelogger;

        public TelegramBotService(
            AppSettings appSettings,
            List<TelegramBotConfigurations> botConfigurations,
            ILogger<TelegramBotService> filelogger)
        {
            this.appSettings = appSettings;
            this.botConfigurations = botConfigurations;

            this.filelogger = filelogger;
        }

        public async Task SetWebhookAsync()
        {
            foreach (var bot in botConfigurations)
            {
                var webhook = $"{appSettings.Domain}/api/telegram/webhook/{bot.BotId}/update";
                await bot.TelegramBotClient.DeleteWebhookAsync();
                await bot.TelegramBotClient.SetWebhookAsync(webhook);
            }
        }

        public async Task<User> GetBotInfoAsync(long botId)
        {
            TelegramBotConfigurations bot = botConfigurations.First(x => x.BotId == botId);
            return await bot.TelegramBotClient.GetMeAsync();
        }

        public async Task<Message> SendTextMessageAsync(string transid, long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2)
        {
            filelogger.LogWarning($"{transid}-Begin send message: botId={botId} chatId={chatId}");
            TelegramBotConfigurations bot = botConfigurations.First(x => x.BotId == botId);
            var message = await bot.BotClient.SendTextMessageAsync(new ChatId(chatId), messageContent, parseMode);
            filelogger.LogWarning($"{transid}-End send message: botId={botId} chatId={chatId}");

            return message;
        }
    }
}