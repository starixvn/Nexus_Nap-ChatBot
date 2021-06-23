namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class PushSignalService : IPushSignalService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly SignalRSettings signalRSettings;
        private readonly ILogger logger;

        public PushSignalService(
            IOptionsSnapshot<SignalRSettings> signalRSettingOptions,
            IHttpClientFactory httpClientFactory,
            ILogger logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.signalRSettings = signalRSettingOptions.Value;
            this.logger = logger;
        }

        public async Task OnInsertNotificationAsync(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                var url = new Uri($"{signalRSettings.DomainURL}{signalRSettings.InsertNotificationURL}");

                var notificationData = new Dictionary<string, string>
                {
                    { "notificationIds", notificationIds }
                };

                var httpClient = httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                using var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(notificationData));

                if (!response.IsSuccessStatusCode)
                {
                    await logger.ErrorAsync($"Send OnInsertNotificationAsync fail - Status Code: {response.StatusCode}");
                }
            }
        }

        public async Task OnUpdateNotificationStatusAsync(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                var token = await GetTokenAsync();
                var url = new Uri($"{signalRSettings.DomainURL}{signalRSettings.UpdateNotificationStatusURL}");

                var notificationData = new Dictionary<string, string>
                {
                    { "notificationIds", notificationIds }
                };

                var httpClient = httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                using var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(notificationData));

                if (!response.IsSuccessStatusCode)
                {
                    await logger.ErrorAsync($"Send OnUpdateNotificationStatusAsync fail - Status Code: {response.StatusCode}");
                }
            }
        }

        public async Task OnHasNewErrNotificationAsync()
        {
            var token = await GetTokenAsync();
            var url = new Uri($"{signalRSettings.DomainURL}{signalRSettings.ErrNotificationURL}");

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            using var response = await httpClient.PostAsync(url, new StringContent(string.Empty));

            if (!response.IsSuccessStatusCode)
            {
                await logger.ErrorAsync($"Send OnHasNewErrNotificationAsync fail - Status Code: {response.StatusCode}");
            }
        }

        protected internal virtual async Task<JwtToken> GetTokenAsync()
        {
            var url = new Uri($"{signalRSettings.DomainURL}{signalRSettings.TokenURL}");
            var authData = new Dictionary<string, string>
            {
                { "username", signalRSettings.AppId },
                { "password", signalRSettings.AppPassword }
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