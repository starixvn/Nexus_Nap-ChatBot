namespace Fanex.NCB.BotService.Core.Services
{
    using Criteria;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Interfaces;
    using Logging.Extensions;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Resources;
    using Shared.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly HttpClient httpClient;
        private readonly AppSettings appSettings;
        private readonly SendIndirectMessageSettings sendIndirectMessageSettings;
        private readonly IEnumerable<TelegramBotConfigurations> botConfigurations;
        private readonly ICommonLogService logService;
        private readonly IConversationService conversationService;

        public TelegramBotService(
            IHttpClientFactory httpClientFactory,
            AppSettings appSettings,
            IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings,
            IOptions<ProxySettings> proxySettings,
            List<TelegramBotConfigurations> botConfigurations,
            ICommonLogService logService,
            IConversationService conversationService)
        {
            this.appSettings = appSettings;
            this.sendIndirectMessageSettings = sendIndirectMessageSettings.Value;
            this.botConfigurations = botConfigurations;
            this.logService = logService;

            if (proxySettings.Value.IsUseProxy)
            {
                httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
                httpClient.Timeout = TimeSpan.FromSeconds(proxySettings.Value.TimeOut);
            }
            else
            {
                httpClient = httpClientFactory.CreateClient();
            }

            foreach (var bot in botConfigurations)
            {
                bot.BotClient = new TelegramBotClient(bot.Token, httpClient);
            }

            this.conversationService = conversationService;
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

#pragma warning disable S2259 //Is null on at least one execution path

        public async Task<User> GetBotInfoAsync(long botId)
        {
            TelegramBotConfigurations bot = botConfigurations.FirstOrDefault(x => x.BotId == botId);
            return await bot?.BotClient.GetMeAsync();
        }

        public async Task<Message> SendTextMessageAsync(long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2)
        {
            TelegramBotConfigurations bot = botConfigurations.FirstOrDefault(x => x.BotId == botId);
            var message = await bot?.BotClient.SendTextMessageAsync(new ChatId(chatId), messageContent, parseMode);
            return message;
        }

        public async Task<SendMessageResult> SendTextMessageAsync(ConversationInfo conversationInfo)
        {
            try
            {
                if (sendIndirectMessageSettings.IsOnlySendIndirect)
                {
                    return await SendIndirectTextMessageAsync(conversationInfo);
                }

                _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
                _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                await SendTextMessageAsync(botId, chatId, conversationInfo.Message.ToMarkdownV2());
            }
            catch (Exception exception)
            {
                await WriteLogAsync(conversationInfo, exception);

                var result = new SendMessageResult { ErrorCode = 1 };
                if (sendIndirectMessageSettings.IsEnabled)
                {
                    result = await SendIndirectTextMessageAsync(conversationInfo);
                }

                return result.ErrorCode == 0 ? result : GetSendMessageResult(conversationInfo, exception);
            }

            return new SendMessageResult
            {
                ConversationId = conversationInfo.ConversationId,
                NotificationId = conversationInfo.NotificationId,
                SentTime = DateTimeOffset.UtcNow.DateTime
            };
        }

#pragma warning restore S2259

        public async Task<SendMessageResult> ResendImageOnlyAsync(ConversationInfo conversationInfo)
        {
            var sendResult = new SendMessageResult
            {
                NotificationId = conversationInfo.NotificationId,
                ConversationId = conversationInfo.ConversationId
            };
            _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
            _ = long.TryParse(conversationInfo.ConversationId, out long chatId);

            foreach (var attachment in conversationInfo.FailedImages)
            {
                await HandleSendImageAsync(botId, chatId, attachment, sendResult);
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationInfo.BotConversationId,
                    ToConversationId = conversationInfo.ConversationId,
                    ReferenceId = conversationInfo.NotificationId,
                    Content = attachment,
                    IsSuccessfull = sendResult.ErrorCode == 0,
                    ErrCode = sendResult.ErrorCode
                };
                await AddImageLogToDatabaseAsync(command);
            }
            return sendResult;
        }

        public async Task<SendMessageResult> SendMessageWithImagesAsync(ConversationInfo conversationInfo)
        {
            var sendResult = await SendTextMessageAsync(conversationInfo);
            await AddMessageLogToDatabaseAsync(conversationInfo, sendResult);
            if (sendResult is not { ErrorCode: 0 } ||
                conversationInfo.Attachments == null || conversationInfo.Attachments.Count <= 0) return sendResult;
            _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
            _ = long.TryParse(conversationInfo.ConversationId, out var chatId);

            foreach (var attachment in conversationInfo.Attachments)
            {
                await HandleSendImageAsync(botId, chatId, attachment, sendResult);
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationInfo.BotConversationId,
                    ToConversationId = conversationInfo.ConversationId,
                    ReferenceId = conversationInfo.NotificationId,
                    Content = attachment,
                    IsSuccessfull = sendResult.ErrorCode == 0,
                    ErrCode = sendResult.ErrorCode
                };
                await AddImageLogToDatabaseAsync(command);
            }
            return sendResult;
        }

        private async Task AddImageLogToDatabaseAsync(AddMessageLogCommand command)
        {
            await conversationService.AddMessageLog(command);
        }

        private async Task AddMessageLogToDatabaseAsync(ConversationInfo conversationInfo, SendMessageResult result)
        {
            try
            {
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationInfo.BotConversationId,
                    ToConversationId = conversationInfo.ConversationId,
                    Content = conversationInfo.Message,
                    ReferenceId = result.NotificationId,
                    IsSuccessfull = result.ErrorCode == 0,
                    ErrorMessage = result.ErrorMessage
                };

                await conversationService.AddMessageLog(command);
            }
            catch (Exception ex)
            {
                await WriteLogAsync(ex);
            }
        }

        private async Task WriteLogAsync(Exception exception, IDictionary<string, object> param = null)
        {
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
            if (param != null)
            {
                logMessage.AddSection("Params", param);
            }

            await logService.SentryErrorAsync(logMessage.ToString());
        }

        private async Task HandleSendImageAsync(long botId, long chatId, string urlImage, SendMessageResult sendMessageResult)
        {
            sendMessageResult.SentTime = DateTimeOffset.UtcNow.DateTime;

            try
            {
                await SendImageAsync(botId, chatId, urlImage);
            }
            catch (Exception ex)
            {
                await WriteLogAsync(ex, new Dictionary<string, object> { { chatId.ToString(), urlImage } });
                sendMessageResult.ErrorCode = 1;
                sendMessageResult.ErrorMessage = ex.Message;
                sendMessageResult.FailedAttachments.Add(urlImage);
            }
        }

        public async Task<Message> SendImageAsync(long botId, long chatId, string urlImage)
        {
            TelegramBotConfigurations bot = botConfigurations.FirstOrDefault(x => x.BotId == botId);

            if (bot is { BotClient: { } })
            {
                var photo = new Telegram.Bot.Types.InputFiles.InputOnlineFile(urlImage);
                Message message = await bot.BotClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: photo);
                return message;
            }

            throw new ArgumentNullException($"{botId} does not existed on config file");
        }

        protected internal virtual async Task<SendMessageResult> SendIndirectTextMessageAsync(ConversationInfo conversationInfo)
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
                result.SentTime = DateTimeOffset.UtcNow.DateTime;

                return result;
            }
            catch (Exception exception)
            {
                var logMessage = new LogMessage(exception.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
                await logService.SentryErrorAsync(logMessage.ToString(), exception);
                return new SendMessageResult
                {
                    ConversationId = conversationInfo.ConversationId,
                    NotificationId = conversationInfo.NotificationId,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message,
                    SentTime = DateTimeOffset.UtcNow.DateTime
                };
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

        protected internal virtual SendMessageResult GetSendMessageResult(ConversationInfo conversationInfo, Exception exception)
        {
            var result = new SendMessageResult
            {
                ConversationId = conversationInfo.ConversationId,
                NotificationId = conversationInfo.NotificationId,
                ErrorCode = 1,
                ErrorMessage = exception.Message,
                SentTime = DateTimeOffset.UtcNow.DateTime
            };
            if (exception.Message.Contains("Message is too long", StringComparison.InvariantCultureIgnoreCase))
            {
                result.ErrorMessage = Resources.MessageIsTooLong;
                result.IsIgnoreRetry = true;
            }

            if (exception.Message.Contains("bot was kicked from the supergroup chat", StringComparison.InvariantCultureIgnoreCase)
                || exception.Message.Contains("bot was kicked from the group chat", StringComparison.InvariantCultureIgnoreCase)
                || exception.Message.Contains("chat not found", StringComparison.InvariantCultureIgnoreCase))
            {
                result.ErrorMessage = Resources.GroupChatDeleted;
                result.IsIgnoreRetry = true;
            }

            return result;
        }

        protected internal virtual async Task WriteLogAsync(ConversationInfo conversationInfo, Exception exception)
        {
            var param = new Dictionary<string, object>
            {
                { "notificationId", conversationInfo.NotificationId },
                { "botId", conversationInfo.BotConversationId },
                { "chatId", conversationInfo.ConversationId },
                { "messageContent", conversationInfo.Message }
            };
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .AddSection("Params", param);

            await logService.SentryErrorAsync(logMessage.ToString());
        }
    }
}