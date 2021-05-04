namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class ChatbotService : IChatbotService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptions<BotSettings> botSettings;
        private readonly IOptions<MonitorChatbot> monitorChatbot;
        private readonly IOptions<NotifyChatGroupOption> notifyChatGroupOptions;

        public ChatbotService(
            IHttpClientFactory httpClientFactory,
            IOptions<BotSettings> botSettings,
            IOptions<MonitorChatbot> monitorChatbot,
            IOptions<NotifyChatGroupOption> notifyChatGroupOptions)
        {
            this.httpClientFactory = httpClientFactory;
            this.monitorChatbot = monitorChatbot;
            this.botSettings = botSettings;
            this.notifyChatGroupOptions = notifyChatGroupOptions;
        }

        public async Task<NotificationMessageResponse> SendMessageToMultitpleAsync(IEnumerable<NotificationMessage> messages)
        {
            var notificationMessageResponse = new NotificationMessageResponse();
            var token = await GetTokenAsync();

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var url = new Uri($"{botSettings.Value.BotDomain}{botSettings.Value.BotServiceUrl}");

            using (var response = await httpClient.PostAsJsonAsync(url, messages))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    notificationMessageResponse.NotificationMessageResult =
                            JsonConvert.DeserializeObject<List<SendMessageResult>>(responseData);
                    notificationMessageResponse.IsSuccessStatusCode = true;
                    notificationMessageResponse.StatusCode = response.StatusCode;
                }
            }

            return notificationMessageResponse;
        }

        private async Task<ChatbotToken> GetTokenAsync()
        {
            var url = new Uri($"{botSettings.Value.BotDomain}{botSettings.Value.AuthenticateUrl}");
            var authData = new Dictionary<string, string>
            {
                { "username", botSettings.Value.AppCredentialId },
                { "password", botSettings.Value.AppCredentialPassword }
            };

            var httpClient = httpClientFactory.CreateClient();
            using var authDataContent = new FormUrlEncodedContent(authData);
            using var response = await httpClient.PostAsync(url, authDataContent);
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ChatbotToken>(responseData);
            return token;
        }

        public async Task MonitorChatbot()
        {
            var settings = monitorChatbot.Value;
            var notificationModel = new List<NotificationMessage>
            {
                new NotificationMessage {
                    NotificationId = -1,
                    Message = settings.Message,
                    Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo
                        {
                            ConversationId = settings.ConversationId,
                            ChannelId = settings.ChannelId,
                            ServiceUrl = settings.ServiceUrl,
                            BotConversationId = settings.BotConversationId,
                            BotName = settings.BotName
                        }
                    }
                }
            };

            await SendMessageToMultitpleAsync(notificationModel);
        }

        public async Task NotifyToChatGroup(string message)
        {
            var settings = notifyChatGroupOptions.Value;
            var notificationModel = new List<NotificationMessage>
                {
                    new NotificationMessage {
                        NotificationId = -1,
                        Message = message,
                        Recipients = new List<ConversationInfo>
                        {
                            new ConversationInfo
                            {
                                ConversationId = settings.ConversationId,
                                ChannelId = settings.ChannelId,
                                ServiceUrl = settings.ServiceUrl,
                                BotConversationId = settings.BotConversationId,
                                BotName = settings.BotName
                            }
                        }
                    }
                };

            await SendMessageToMultitpleAsync(notificationModel);
        }

        public IEnumerable<NotificationMessage> GenerateNotifificationMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var messages = new List<NotificationMessage>();
            if (notifications != null && notifications.Any())
            {
                foreach (var noti in notifications)
                {
                    var notification = new NotificationMessage
                    {
                        NotificationId = noti.NotificationId,
                        Message = noti.Content,
                        Recipients = recipients.Select(r =>
                            new ConversationInfo
                            {
                                BotConversationId = r.BotConversationId,
                                BotName = r.BotName,
                                ChannelId = r.ChannelId,
                                ConversationId = r.ChatGroupConversationId,
                                ServiceUrl = r.ServiceUrl
                            }
                        )
                    };
                    messages.Add(notification);
                }
            }

            return messages;
        }
    }
}