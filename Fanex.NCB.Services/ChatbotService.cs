namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
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
    using RestSharp;

    public class ChatbotService : IChatbotService
    {
        private readonly IRestClient webClient;
        private readonly ILogService logService;
        private readonly IOptionsSnapshot<BotSettings> botSettings;
        private readonly IOptionsSnapshot<MonitorChatbot> monitorChatbot;
        private readonly IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions;
        private readonly IOptionsSnapshot<List<HangfireJobSetting>> jobSettings;

        public ChatbotService(
            IRestClient webClient,
            ILogService logService,
            IOptionsSnapshot<BotSettings> botSettings,
            IOptionsSnapshot<MonitorChatbot> monitorChatbot,
            IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions,
            IOptionsSnapshot<List<HangfireJobSetting>> jobSettings)
        {
            this.webClient = webClient;
            this.logService = logService;
            this.monitorChatbot = monitorChatbot;
            this.botSettings = botSettings;
            this.jobSettings = jobSettings;
            this.notifyChatGroupOptions = notifyChatGroupOptions;
        }

        public async Task<IRestResponse> SendMessageToMultitpleAsync(IEnumerable<NotificationMessage> messages)
        {
            ChatbotToken token = await GetTokenAsync();
            webClient.BaseUrl = new Uri(botSettings.Value.BotDomain);
            var request = new RestRequest(botSettings.Value.BotServiceUrl, Method.POST);
            request.AddHeader("Authorization", $"Bearer {token?.AccessToken}");
            request.AddJsonBody(messages);

            return await webClient.ExecutePostTaskAsync(request);
        }

        public async Task SendMessageAsync(string botName, string conversationId, string message)
        {
            if (!string.IsNullOrWhiteSpace(conversationId))
            {
                ChatbotToken token = await GetTokenAsync();

                await ForwardToBotAsync(botName, message, conversationId, token);
            }
        }

        private async Task ForwardToBotAsync(string botName, string message, string conversationId, ChatbotToken token)
        {
            webClient.BaseUrl = new Uri(botSettings.Value.BotDomain);

            var request = new RestRequest(botSettings.Value.SendSingleMessage, Method.POST);
            request.AddHeader("Authorization", $"{token?.TokenType} {token?.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("botName", botName);
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostTaskAsync(request);
        }

        private async Task<ChatbotToken> GetTokenAsync()
        {
            webClient.BaseUrl = new Uri(botSettings.Value.BotDomain);
            var request = new RestRequest(botSettings.Value.AuthenticateUrl, Method.POST);
            request.AddParameter("username", botSettings.Value.AppCredentialId);
            request.AddParameter("password", botSettings.Value.AppCredentialPassword);

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostTaskAsync<ChatbotToken>(request);
            if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
            {
                await logService.WriteExceptionSentryErrorAsync(response.ErrorException);
            }
            return response.Data;
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