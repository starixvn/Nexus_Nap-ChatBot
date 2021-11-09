namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.Resources;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly HttpClient httpClient;
        private readonly AppSettings appSettings;
        private readonly SendIndirectMessageSettings sendIndirectMessageSettings;
        private readonly IEnumerable<TelegramBotConfigurations> botConfigurations;
        private readonly Logging.ILogger logger;

        public TelegramBotService(
            IHttpClientFactory httpClientFactory,
            AppSettings appSettings,
            IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings,
            IOptions<ProxySettings> proxySettings,
            List<TelegramBotConfigurations> botConfigurations,
            Logging.ILogger logger)
        {
            this.appSettings = appSettings;
            this.sendIndirectMessageSettings = sendIndirectMessageSettings.Value;
            this.botConfigurations = botConfigurations;
            this.logger = logger;
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
                await bot.BotClient.DeleteWebhookAsync();
                await bot.BotClient.SetWebhookAsync(webhook);
            }
        }

        public async Task<User> GetBotInfoAsync(long botId)
        {
            TelegramBotConfigurations bot = botConfigurations.First(x => x.BotId == botId);
            return await bot.BotClient.GetMeAsync();
        }

        public async Task<Message> SendTextMessageAsync(long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2)
        {
            TelegramBotConfigurations bot = botConfigurations.First(x => x.BotId == botId);
            var message = await bot.BotClient.SendTextMessageAsync(new ChatId(chatId), messageContent, parseMode);
            return message;
        }

        public async Task<SendMessageResult> SendTextMessageAsync(ConversationInfo conversationInfo)
        {
            return sendIndirectMessageSettings.IsEnabled
                ? await SendIndirectTextMessageAsync(conversationInfo)
                : await SendDirectTextMessageAsync(conversationInfo);
        }

        protected internal virtual async Task<SendMessageResult> SendDirectTextMessageAsync(ConversationInfo conversationInfo)
        {
            try
            {
                _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
                _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                await SendTextMessageAsync(botId, chatId, conversationInfo.Message.ToMarkdownV2());
            }
            catch (Exception exception)
            {
                return await HandleExceptionAsync(conversationInfo, exception);
            }

            return new SendMessageResult
            {
                ConversationId = conversationInfo.ConversationId,
                NotificationId = conversationInfo.NotificationId,
                SentTime = DateTime.UtcNow
            };
        }

        public async Task<SendMessageResult> SendIndirectTextMessageAsync(ConversationInfo conversationInfo)
        {
            try
            {
                var token = await GetTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var url = new Uri($"{sendIndirectMessageSettings.TelegramUrl}");
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, conversationInfo);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendMessageResult>(responseData);

                result.ConversationId = conversationInfo.ConversationId;
                result.NotificationId = conversationInfo.NotificationId;
                result.SentTime = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                {
                    if (result.ErrorMessage.Contains("Message is too long", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.ErrorMessage = Resources.MessageIsTooLong;
                        result.IsIgnoreRetry = true;
                    }

                    if (result.ErrorMessage.Contains("bot was kicked from the supergroup chat", StringComparison.InvariantCultureIgnoreCase)
                        || result.ErrorMessage.Contains("chat not found", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.ErrorMessage = Resources.GroupChatDeleted;
                        result.IsIgnoreRetry = true;
                    }
                }

                return result;
            }
            catch(Exception exception)
            {
                return await HandleExceptionAsync(conversationInfo, exception);
            }
        }

        protected internal virtual async Task<ChatbotToken> GetTokenAsync()
        {
            var url = new Uri($"{sendIndirectMessageSettings.AuthenticateUrl}");
            var authData = new Dictionary<string, string>
            {
                { "username", sendIndirectMessageSettings.AppCredentialId },
                { "password", sendIndirectMessageSettings.AppCredentialPassword }
            };
            using var authDataContent = new FormUrlEncodedContent(authData);
            HttpResponseMessage response = await httpClient.PostAsync(url, authDataContent);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ChatbotToken>(responseData);
            return token;
        }

        protected internal virtual async Task<SendMessageResult> HandleExceptionAsync(ConversationInfo conversationInfo, Exception exception)
        {
            var param = new Dictionary<string, object>
            {
                { "notificationId", conversationInfo.NotificationId },
                { "botId", conversationInfo.BotConversationId },
                { "chatId", conversationInfo.ConversationId },
                { "messageContent", conversationInfo.Message }
            };
            await WriteLogAsync(exception, param);

            var result = new SendMessageResult
            {
                ConversationId = conversationInfo.ConversationId,
                NotificationId = conversationInfo.NotificationId,
                ErrorCode = 1,
                ErrorMessage = exception.Message,
                SentTime = DateTime.UtcNow
            };
            if (exception.Message.Contains("Message is too long", StringComparison.InvariantCultureIgnoreCase))
            {
                result.ErrorMessage = Resources.MessageIsTooLong;
                result.IsIgnoreRetry = true;
            }

            if (exception.Message.Contains("bot was kicked from the supergroup chat", StringComparison.InvariantCultureIgnoreCase)
                || exception.Message.Contains("chat not found", StringComparison.InvariantCultureIgnoreCase))
            {
                result.ErrorMessage = Resources.GroupChatDeleted;
                result.IsIgnoreRetry = true;
            }

            return result;
        }

        protected internal virtual async Task WriteLogAsync(Exception exception, IDictionary<string, object> param = null)
        {
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
            if (param != null)
            {
                logMessage.AddSection("Params", param);
            }

            await logger.ErrorAsync(logMessage.ToString());
        }
    }
}