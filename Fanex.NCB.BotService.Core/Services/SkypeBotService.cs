namespace Fanex.NCB.BotService.Core.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISkypeBotService
    {
        Task SendMessageAsync(ConversationInfo conversationInfo);

        Task<SendMessageResult> SendMessageWithRetryAsync(ConversationInfo conversationInfo);
    }

    public class SkypeBotService : ISkypeBotService
    {
        private readonly HttpClient httpClient;
        private readonly SendIndirectMessageSettings sendIndirectMessageSettings;
        private readonly ICommonLogService logService;
        private readonly IAdapterIntegration adapter;
#pragma warning disable CS0618 // Type or member is obsolete
        private readonly BotConfiguration botConfig;
#pragma warning restore CS0618 // Type or member is obsolete

        public SkypeBotService(
            IHttpClientFactory httpClientFactory,
            IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings,
            IOptions<ProxySettings> proxySettings,
            ICommonLogService logService,
            IAdapterIntegration adapter,
#pragma warning disable CS0618 // Type or member is obsolete
            BotConfiguration botConfig)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            this.sendIndirectMessageSettings = sendIndirectMessageSettings.Value;
            this.logService = logService;
            this.adapter = adapter;
            this.botConfig = botConfig;

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

        public async Task SendMessageAsync(ConversationInfo conversationInfo)
        {
            var appId = GetAppId(conversationInfo.BotName);
            var conversationReference = GetConversationReference(conversationInfo);
            await adapter.ContinueConversationAsync(appId, conversationReference,
                async (context, token) =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    AppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
#pragma warning restore CS0618 // Type or member is obsolete
                    var activity = MessageFactory.Text(conversationInfo.Message);
                    if (conversationInfo.Attachments != null && conversationInfo.Attachments.Count > 0)
                    {
                        foreach (var attachment in conversationInfo.Attachments)
                        {
                            activity.Attachments.Add(new Attachment
                            {
                                ContentType = "image",
                                ContentUrl = attachment
                            });
                        }
                    }
                    await context.SendActivityAsync(activity, cancellationToken: token);
                },
                new CancellationToken());
        }

        public async Task<SendMessageResult> SendMessageWithRetryAsync(ConversationInfo conversationInfo)
        {
            logService.LogFileMessage($"SendMessageWithRetryAsync start: {JsonConvert.SerializeObject(conversationInfo)}");
            try
            {
                if (sendIndirectMessageSettings.IsOnlySendIndirect)
                {
                    return await SendIndirectMessageAsync(conversationInfo);
                }
                var conversationReference = GetConversationReference(conversationInfo);
                var appId = GetAppId(conversationInfo.BotName);

                await adapter.ContinueConversationAsync(appId, conversationReference,
                    async (context, token) =>
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        AppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
#pragma warning restore CS0618 // Type or member is obsolete
                        var activity = MessageFactory.Text(conversationInfo.Message);
                        if (conversationInfo.Attachments != null && conversationInfo.Attachments.Count > 0)
                        {
                            var attachments = conversationInfo.Attachments.Select(img =>
                            {
                                return new Attachment
                                {
                                    ContentType = "image",
                                    ContentUrl = img
                                };
                            }).ToList();
                            activity.Attachments = attachments;
                        }

                        await context.SendActivityAsync(activity, cancellationToken: token);
                    },
                    new CancellationToken());
            }
            catch (Exception exception)
            {
                await WriteLogAsync(conversationInfo, exception);

                var result = new SendMessageResult { ErrorCode = 1 };
                if (sendIndirectMessageSettings.IsEnabled)
                {
                    result = await SendIndirectMessageAsync(conversationInfo);
                }

                return result.ErrorCode == 0 ? result : new SendMessageResult
                {
                    ConversationId = conversationInfo.ConversationId,
                    NotificationId = conversationInfo.NotificationId,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message,
                    SentTime = DateTimeOffset.UtcNow.DateTime
                };
            }

            logService.LogFileMessage("SendMessageWithRetryAsync: end");

            return new SendMessageResult
            {
                ConversationId = conversationInfo.ConversationId,
                NotificationId = conversationInfo.NotificationId,
                SentTime = DateTimeOffset.UtcNow.DateTime
            };
        }

        private string GetAppId(string botName)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var endpoint = (EndpointService)botConfig.Services
                .Find(t => t.Name.Equals(botName, StringComparison.InvariantCultureIgnoreCase));
#pragma warning restore CS0618 // Type or member is obsolete
            string appId;
            if (endpoint != null && !string.IsNullOrEmpty(endpoint.AppId))
            {
                appId = endpoint.AppId;
            }
            else
            {
                appId = Guid.NewGuid().ToString();
                logService.SentryError($"BotName : {botName} is not supported by system.");
            }

            return appId;
        }

        private static ConversationReference GetConversationReference(ConversationInfo conversationInfo)
        {
            var coversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = conversationInfo.BotConversationId, Name = conversationInfo.BotName, Role = "bot" },
                Conversation = new ConversationAccount { Id = conversationInfo.ConversationId },
                ServiceUrl = conversationInfo.ServiceUrl,
                ChannelId = conversationInfo.ChannelId
            };

            return coversation;
        }

        protected internal virtual async Task<SendMessageResult> SendIndirectMessageAsync(ConversationInfo conversationInfo)
        {
            try
            {
                var token = await GetTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var url = new Uri($"{sendIndirectMessageSettings.SkypeUrl}");
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
                logService.LogFileMessage(logMessage.ToString());

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

        protected internal virtual async Task WriteLogAsync(ConversationInfo conversationInfo, Exception exception)
        {
            var param = new Dictionary<string, object>
            {
                { "notificationId", conversationInfo.NotificationId },
                { "botId", conversationInfo.BotConversationId },
                { "chatId", conversationInfo.ConversationId },
                { "messageContent", conversationInfo.Message },
                { "attachments", conversationInfo.Attachments }
            };
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .AddSection("Params", param);

            await logService.SentryErrorAsync(logMessage.ToString());
        }
    }
}