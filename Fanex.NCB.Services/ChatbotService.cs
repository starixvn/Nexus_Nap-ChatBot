namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class ChatbotService : IChatbotService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptionsSnapshot<BotSettings> botSettings;
        private readonly IOptionsSnapshot<MonitorChatbot> monitorChatbot;
        private readonly IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions;
        private readonly IOptionsSnapshot<List<HangfireJobSetting>> jobSettings;

        public ChatbotService(
            IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<BotSettings> botSettings,
            IOptionsSnapshot<MonitorChatbot> monitorChatbot,
            IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions,
            IOptionsSnapshot<List<HangfireJobSetting>> jobSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.monitorChatbot = monitorChatbot;
            this.botSettings = botSettings;
            this.jobSettings = jobSettings;
            this.notifyChatGroupOptions = notifyChatGroupOptions;
        }

        public async Task<HttpResponseMessage> SendMessageToMultitpleAsync(IEnumerable<NotificationMessage> messages)
        {
            var token = await GetTokenAsync();
            var messagesJson = JsonConvert.SerializeObject(messages,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var url = new Uri($"{botSettings.Value.BotDomain}{botSettings.Value.BotServiceUrl}");

            var content = new StringContent(messagesJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);

            return response;
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
            var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(authData));
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ChatbotToken>(responseData);
            return token;
        }

        public async Task MonitorChatbot()
        {
            if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.MonitorSkypebot))
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