namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public ChatbotService(IOptionsSnapshot<BotSettings> botSettings)
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            this.botSettings = botSettings;
        }

        public async Task<IRestResponse> SendMessageToMultitpleAsync(IEnumerable<ProactiveMessage> proactiveMessages)
        {
            webClient.BaseUrl = new Uri(botSettings.Value.BotDomain);

            var request = new RestRequest(botSettings.Value.BotServiceUrl, Method.POST);
            request.AddJsonBody(proactiveMessages);

            return await webClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
        }

        public async Task SendMessageAsync(string botName, string conversationId, string message)
        {
            if (!string.IsNullOrWhiteSpace(botName) && !string.IsNullOrWhiteSpace(conversationId))
            {
                ChatbotToken token = await GetTokenAsync(botName).ConfigureAwait(false);
                await ForwardToBotAsync(botName, message, conversationId, token).ConfigureAwait(false);
            }
        }

        private async Task ForwardToBotAsync(string botName, string message, string conversationId, ChatbotToken token)
        {
            webClient.BaseUrl = new Uri(botSettings.Value.BotDomain);

            var request = new RestRequest(botSettings.Value.BotServiceUrl, Method.POST);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("botName", botName);
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
        }

        private async Task<ChatbotToken> GetTokenAsync(string botName)
        {
            webClient.BaseUrl = new Uri(botSettings.Value.TokenUrl);
            var botSelected = botSettings.Value.Bots.FirstOrDefault(bot => bot.Name.Equals(botName));
            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", botSelected.AppId);
            request.AddParameter("client_secret", botSelected.AppPassword);
            request.AddParameter("scope", $"{botSelected.AppId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostTaskAsync<ChatbotToken>(request).ConfigureAwait(false);

            return response.Data;
        }
    }
}