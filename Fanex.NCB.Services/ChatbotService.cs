namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using RestSharp;

    public class ChatbotService : IChatbotService
    {
        private readonly IRestClient webClient;
        private readonly IOptionsSnapshot<BotSettings> botSettings;
        private readonly IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions;

        public ChatbotService(
            IOptionsSnapshot<BotSettings> botSettings,
            IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions)
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            this.notifyChatGroupOptions = notifyChatGroupOptions;
            this.botSettings = botSettings;
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

            return response.Data;
        }

        public async Task NotifyToGroup(string message)
        {
            var settings = notifyChatGroupOptions.Value;
            var notificationModel = new List<NotificationMessage>
                    {
                        new NotificationMessage {
                            NotifictaionId = -1,
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
    }
}