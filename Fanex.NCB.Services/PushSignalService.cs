namespace Fanex.NCB.Services
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using RestSharp;

    public class PushSignalService : IPushSignalService
    {
        private readonly IRestClient restClient;
        private readonly IOptionsSnapshot<SignalRSettings> signalRSettings;
        private const string ContentTypeName = "content-type";
        private const string ContentTypeValue = "application/x-www-form-urlencoded";

        public PushSignalService(IOptionsSnapshot<SignalRSettings> signalRSettings)
        {
            this.restClient = new RestClient { Encoding = Encoding.UTF8 };
            this.signalRSettings = signalRSettings;
        }

        public async Task OnInsertNotification(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                restClient.BaseUrl = new Uri(signalRSettings.Value.DomainURL);

                var request = new RestRequest(signalRSettings.Value.InsertNotificationURL, Method.POST);
                request.AddHeader("Authorization", $"{token?.TokenType ?? "Bearer"} {token?.AccessToken}");
                request.AddParameter("notificationIds", notificationIds);
                request.AddHeader(ContentTypeName, ContentTypeValue);

                await restClient.ExecutePostTaskAsync(request);
            }
        }

        public async Task OnUpdateNotificationStatus(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                restClient.BaseUrl = new Uri(signalRSettings.Value.DomainURL);

                var request = new RestRequest(signalRSettings.Value.UpdateNotificationStatusURL, Method.POST);
                request.AddHeader("Authorization", $"{token?.TokenType ?? "Bearer"} {token?.AccessToken}");
                request.AddParameter("notificationIds", notificationIds);
                request.AddHeader(ContentTypeName, ContentTypeValue);

                await restClient.ExecutePostTaskAsync(request);
            }
        }

        public async Task OnHasNewErrNotifications()
        {
            var token = await GetTokenAsync();
            restClient.BaseUrl = new Uri(signalRSettings.Value.DomainURL);

            var request = new RestRequest(signalRSettings.Value.ErrNotificationURL, Method.POST);
            request.AddHeader("Authorization", $"{token?.TokenType ?? "Bearer"} {token?.AccessToken}");
            request.AddHeader(ContentTypeName, ContentTypeValue);

            await restClient.ExecutePostTaskAsync(request);
        }

        private async Task<JwtToken> GetTokenAsync()
        {
            restClient.BaseUrl = new Uri(signalRSettings.Value.DomainURL);
            var request = new RestRequest(signalRSettings.Value.TokenURL, Method.POST);
            request.AddHeader(ContentTypeName, ContentTypeValue);
            request.AddParameter("username", signalRSettings.Value.AppId);
            request.AddParameter("password", signalRSettings.Value.AppPassword);

            var response = await restClient.ExecutePostTaskAsync<JwtToken>(request);

            return response.Data;
        }
    }
}