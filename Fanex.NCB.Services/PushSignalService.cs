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
        private readonly IOptionsSnapshot<SignalRSettings> signalRSettings;
        private readonly ILogger logger;

        public PushSignalService(
            IOptionsSnapshot<SignalRSettings> signalRSettings, 
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
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(notificationData));

                if (response.IsSuccessStatusCode)
                {
                    await WriteLog(response);
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

                var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(notificationData));

                if (response.IsSuccessStatusCode)
                {
                    await WriteLog(response);
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

            var response = await httpClient.PostAsync(url, new StringContent(""));

            if (response.IsSuccessStatusCode)
            {
                await WriteLog(response);
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
            var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(authData));
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(responseData);

            return token;
        }

        private async Task WriteLog(HttpResponseMessage response)
        {

            var logMessage = JsonConvert.SerializeObject(response.Content);
            await logger.ErrorAsync(logMessage);
        }
    }
}