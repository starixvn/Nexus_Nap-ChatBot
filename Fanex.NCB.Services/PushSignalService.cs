namespace Fanex.NCB.Services
{
    using Fanex.Logging;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class PushSignalService : IPushSignalService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptions<SignalRSettings> signalRSettings;
        private readonly ILogger logger;

        public PushSignalService(
            IOptions<SignalRSettings> signalRSettings, 
            IHttpClientFactory httpClientFactory, 
            ILogger logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.signalRSettings = signalRSettings;
            this.logger = logger;
        }

        public async Task OnInsertNotification(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                var url = new Uri($"{signalRSettings.Value.DomainURL}{signalRSettings.Value.InsertNotificationURL}");

                var notificationData = new Dictionary<string, string>
                {
                    { "notificationIds", notificationIds }
                };

                var httpClient = httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                using var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(notificationData));

                if (!response.IsSuccessStatusCode)
                {
                    await logger.ErrorAsync($"Send OnInsertNotification fail - Status Code: {response.StatusCode}");
                }
            }
        }

        public async Task OnUpdateNotificationStatus(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                var url = new Uri($"{signalRSettings.Value.DomainURL}{signalRSettings.Value.UpdateNotificationStatusURL}");

                var notificationData = new Dictionary<string, string>
                {
                    { "notificationIds", notificationIds }
                };

                var httpClient = httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                using var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(notificationData));

                if (!response.IsSuccessStatusCode)
                {
                    await logger.ErrorAsync($"Send OnUpdateNotificationStatus fail - Status Code: {response.StatusCode}");
                }
            }
        }

        public async Task OnHasNewErrNotifications()
        {
            var token = await GetTokenAsync();
            var url = new Uri($"{signalRSettings.Value.DomainURL}{signalRSettings.Value.ErrNotificationURL}");

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            using var response = await httpClient.PostAsync(url, new StringContent(string.Empty));

            if (!response.IsSuccessStatusCode)
            {
                await logger.ErrorAsync($"Send OnHasNewErrNotifications fail - Status Code: {response.StatusCode}");
            }
        }

        private async Task<JwtToken> GetTokenAsync()
        {
            var url = new Uri($"{signalRSettings.Value.DomainURL}{signalRSettings.Value.TokenURL}");
            var authData = new Dictionary<string, string>
            {
                { "username", signalRSettings.Value.AppId },
                { "password", signalRSettings.Value.AppPassword }
            };

            var httpClient = httpClientFactory.CreateClient();
            using var authDataContent = new FormUrlEncodedContent(authData);
            using var response = await httpClient.PostAsync(url, authDataContent);
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(responseData);

            return token;
        }
    }
}