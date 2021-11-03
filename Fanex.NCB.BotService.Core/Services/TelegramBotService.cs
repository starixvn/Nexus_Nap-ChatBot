namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Models;
    using Microsoft.Extensions.Options;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly HttpClient httpClient;
        private readonly AppSettings appSettings;
        private readonly IEnumerable<TelegramBotConfigurations> botConfigurations;

        public TelegramBotService(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> appSettings,
            IOptions<ProxySettings> proxySettings,
            IOptions<List<TelegramBotConfigurations>> botConfigurations)
        {
            this.appSettings = appSettings.Value;
            this.botConfigurations = botConfigurations.Value;
            if (proxySettings.Value.IsUseProxy)
            {
                httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
                httpClient.Timeout = TimeSpan.FromSeconds(proxySettings.Value.TimeOut);
            }
            else
            {
                httpClient = httpClientFactory.CreateClient();
            }
        }

        public async Task SetWebhookAsync()
        {
            foreach (var bot in botConfigurations)
            {
                var webhook = $"{appSettings.Domain}/api/telegram/webhook/{bot.BotId}/update";
                var botClient = new TelegramBotClient(bot.Token, httpClient);
                await botClient.DeleteWebhookAsync();
                await botClient.SetWebhookAsync(webhook);
            }
        }

        public async Task<User> GetBotInfoAsync(long botId)
        {
            TelegramBotConfigurations bot = botConfigurations.First(x => x.BotId == botId);
            var botClient = new TelegramBotClient(bot.Token, httpClient);
            return await botClient.GetMeAsync();
        }

        public async Task<Message> SendTextMessageAsync(long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2)
        {
            TelegramBotConfigurations bot = botConfigurations.First(x => x.BotId == botId);
            var botClient = new TelegramBotClient(bot.Token, httpClient);
            return await botClient.SendTextMessageAsync(new ChatId(chatId), messageContent, parseMode);
        }
    }
}