namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public interface ISkypeBotService
    {
        Task SendMessageAsync(ConversationInfo conversationInfo);

        Task<SendMessageResult> SendMessageWithRetryAsync(ConversationInfo conversationInfo);
    }

    public class SkypeBotService : ISkypeBotService
    {
        private readonly HttpClient httpClient;
        private readonly SendIndirectMessageSettings sendIndirectMessageSettings;
        private readonly ILogger<SkypeBotService> filelogger;
        private readonly Logging.ILogger logger;
        private readonly IAdapterIntegration adapter;

        [Obsolete]
        private readonly BotConfiguration botConfig;

        [Obsolete]
        public SkypeBotService(
            IHttpClientFactory httpClientFactory,
            IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings,
            IOptions<ProxySettings> proxySettings,
            ILogger<SkypeBotService> filelogger,
            Logging.ILogger logger,
            IAdapterIntegration adapter,
            BotConfiguration botConfig)
        {
            this.sendIndirectMessageSettings = sendIndirectMessageSettings.Value;
            this.filelogger = filelogger;
            this.logger = logger;
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

        [Obsolete]
        public async Task SendMessageAsync(ConversationInfo conversationInfo)
        {
            var appId = GetAppId(conversationInfo.BotName);
            var conversationReference = GetConversationReference(conversationInfo);
            await adapter.ContinueConversationAsync(appId, conversationReference,
                async (context, token) =>
                {
                    AppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
                    await context.SendActivityAsync(conversationInfo.Message, cancellationToken: token);
                },
                new CancellationToken());
        }


        [Obsolete]
        public async Task<SendMessageResult> SendMessageWithRetryAsync(ConversationInfo conversationInfo)
        {
            try
            {
                var conversationReference = GetConversationReference(conversationInfo);
                var appId = GetAppId(conversationInfo.BotName);
                await adapter.ContinueConversationAsync(appId, conversationReference,
                    async (context, token) =>
                    {
                        AppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
                        await context.SendActivityAsync(conversationInfo.Message, cancellationToken: token);
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
                        SentTime = DateTime.UtcNow
                    };
            }

            return new SendMessageResult
            {
                ConversationId = conversationInfo.ConversationId,
                NotificationId = conversationInfo.NotificationId,
                SentTime = DateTime.UtcNow
            };
        }

        [Obsolete]
        private string GetAppId(string botName)
        {
            var endpoint = (EndpointService)botConfig.Services
                .FirstOrDefault(t => t.Name.Equals(botName, StringComparison.InvariantCultureIgnoreCase));
            string appId;
            if (endpoint != null && !string.IsNullOrEmpty(endpoint.AppId))
            {
                appId = endpoint.AppId;
            }
            else
            {
                appId = Guid.NewGuid().ToString();
                logger.Error($"BotName : { botName } is not supported by system.");
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
                result.SentTime = DateTime.UtcNow;

                return result;
            }
            catch (Exception exception)
            {
                var logMessage = new LogMessage(exception.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
                filelogger.LogWarning(logMessage.ToString());

                return new SendMessageResult
                {
                    ConversationId = conversationInfo.ConversationId,
                    NotificationId = conversationInfo.NotificationId,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message,
                    SentTime = DateTime.UtcNow
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
                { "messageContent", conversationInfo.Message }
            };
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .AddSection("Params", param);

            await logger.ErrorAsync(logMessage.ToString());
        }
    }
}