namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
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
        private readonly BotSettings botSettings;
        private readonly MonitorChatbot monitorChatbot;
        private readonly NotifyChatGroupOption notifyChatGroupOptions;
        private readonly MonitorTelegramBot monitorTelegramBot;

        public ChatbotService(
            IHttpClientFactory httpClientFactory,
            IOptions<BotSettings> botSettings,
            IOptions<MonitorChatbot> monitorChatbot,
            IOptions<NotifyChatGroupOption> notifyChatGroupOptions,
            IOptions<MonitorTelegramBot> monitorTelegramBot)
        {
            this.httpClientFactory = httpClientFactory;
            this.monitorChatbot = monitorChatbot.Value;
            this.botSettings = botSettings.Value;
            this.notifyChatGroupOptions = notifyChatGroupOptions.Value;
            this.monitorTelegramBot = monitorTelegramBot.Value;
        }

        public async Task<NotificationMessageResponse> SendMultiMessageAsync(byte channelId, IEnumerable<NotificationMessage> messages)
        {
            var messageResponse = new NotificationMessageResponse();
            if (messages == null || !messages.Any())
            {
                return messageResponse;
            }

            BotServiceEndPointConfigs apiConfigs = botSettings.ApiConfigs.First(x => x.ChannelId == channelId);
            var token = await GetTokenAsync();

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var url = new Uri($"{botSettings.BotDomain}{apiConfigs.SendMultiMessageUrl}");
            using (var response = await httpClient.PostAsJsonAsync(url, messages))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    messageResponse.NotificationMessageResult = JsonConvert.DeserializeObject<List<SendMessageResult>>(responseData);
                    messageResponse.IsSuccessStatusCode = true;
                    messageResponse.StatusCode = response.StatusCode;
                }
            }

            return messageResponse;
        }

        public async Task MonitorChatbotAsync()
        {
            var conversationInfo = new ConversationInfo
            {
                ConversationId = monitorChatbot.ConversationId,
                ChannelId = monitorChatbot.ChannelId,
                BotName = monitorChatbot.BotName,
                ServiceUrl = monitorChatbot.ServiceUrl,
                BotConversationId = monitorChatbot.BotConversationId,
                Message = monitorChatbot.Message
            };
            await SendSingleMessageAsync((byte)ConversationChannel.Skype, conversationInfo);
        }

        public async Task MonitorTelegramBotAsync()
        {
            var conversationInfo = new ConversationInfo
            {
                ConversationId = monitorTelegramBot.ChatId.ToString(),
                BotConversationId = monitorTelegramBot.BotId.ToString(),
                Message = monitorTelegramBot.Message
            };
            await SendSingleMessageAsync((byte)ConversationChannel.Telegram, conversationInfo);
        }

        public async Task NotifyToChatGroupAsync(string message)
        {
            var notifications = new List<NotificationMessage>
                {
                    new NotificationMessage {
                        NotificationId = -1,
                        Message = message,
                        Recipients = new List<ConversationInfo>
                        {
                            new ConversationInfo
                            {
                                ConversationId = notifyChatGroupOptions.ConversationId,
                                ChannelId = notifyChatGroupOptions.ChannelId,
                                ServiceUrl = notifyChatGroupOptions.ServiceUrl,
                                BotConversationId = notifyChatGroupOptions.BotConversationId,
                                BotName = notifyChatGroupOptions.BotName
                            }
                        }
                    }
                };

            await SendMultiMessageAsync((byte)ConversationChannel.Skype, notifications);
        }

        public IEnumerable<NotificationMessage> GenerateNotifificationMessageForCarbonGroupAsync(
            IEnumerable<NotificationSendMessage> notifications,
            IEnumerable<NotificationRecipient> recipients)
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
                        ChannelId = noti.ChannelId,
                        Recipients = recipients.Where(r => r.ChannelId == noti.ChannelId).Select(r =>
                            new ConversationInfo
                            {
                                BotConversationId = r.BotConversationId,
                                BotName = r.BotName,
                                ChannelId = r.ChannelName,
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

        protected internal virtual async Task<HttpResponseMessage> SendSingleMessageAsync(byte channelId, ConversationInfo conversationInfo)
        {
            BotServiceEndPointConfigs apiConfigs = botSettings.ApiConfigs.First(x => x.ChannelId == channelId);
            var token = await GetTokenAsync();

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var url = new Uri($"{botSettings.BotDomain}{apiConfigs.SendMessageUrl}");
            return await httpClient.PostAsJsonAsync(url, conversationInfo);
        }

        protected internal virtual async Task<ChatbotToken> GetTokenAsync()
        {
            var url = new Uri($"{botSettings.BotDomain}{botSettings.AuthenticateUrl}");
            var authData = new Dictionary<string, string>
            {
                { "username", botSettings.AppCredentialId },
                { "password", botSettings.AppCredentialPassword }
            };

            var httpClient = httpClientFactory.CreateClient();
            using var authDataContent = new FormUrlEncodedContent(authData);
            using var response = await httpClient.PostAsync(url, authDataContent);
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ChatbotToken>(responseData);
            return token;
        }
    }
}