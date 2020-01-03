namespace Fanex.NCB.AutoJobs.Services
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Models;
    using Microsoft.Extensions.Options;
    using RestSharp;

    public class PushNotifyService : IPushNotifyService
    {
        private readonly IRestClient restClient;
        private readonly IOptionsSnapshot<SignalRSettings> signalRSettings;

        public PushNotifyService(IRestClient restClient, IOptionsSnapshot<SignalRSettings> signalRSettings)
        {
            this.restClient = new RestClient { Encoding = Encoding.UTF8 };
            this.signalRSettings = signalRSettings;
        }

        public async Task PushNotificationUpdatedStatus(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                restClient.BaseUrl = new Uri(signalRSettings.Value.DomainURL);

                var request = new RestRequest(signalRSettings.Value.UpdateNotificationStatusURL, Method.POST);
                request.AddHeader("Authorization", $"{token?.TokenType} {token?.AccessToken}");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("notificationIds", notificationIds);

                await restClient.ExecutePostTaskAsync(request);
            }
        }

        private async Task<ChatbotToken> GetTokenAsync()
        {
            restClient.BaseUrl = new Uri(signalRSettings.Value.DomainURL);
            var request = new RestRequest(signalRSettings.Value.TokenURL, Method.POST);
            request.AddParameter("username", signalRSettings.Value.AppId);
            request.AddParameter("password", signalRSettings.Value.AppPassword);

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await restClient.ExecutePostTaskAsync<ChatbotToken>(request);

            return response.Data;
        }
    }
}