namespace Fanex.NCB.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Http.Connections;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Options;

    public class PushSignalRService : IPushSignalRService
    {
        private readonly HubConnection hubConnection;
        private readonly ILogService logger;
        private readonly INotificationService notificationService;
        private readonly SignalRSettings signalRSettings;

        public PushSignalRService(ILogService logger, IOptions<SignalRSettings> signalRSettings, INotificationService notificationService)
        {
            this.logger = logger;
            this.signalRSettings = signalRSettings.Value;
            this.notificationService = notificationService;
            hubConnection = new HubConnectionBuilder()
                          .WithUrl(new Uri(signalRSettings.Value.HubEndpoint),
                               options =>
                               {
                                   options.Transports = HttpTransportType.WebSockets;
                                   options.SkipNegotiation = true;
                               })
                          .WithAutomaticReconnect()
                          .Build();
        }

        public async Task PushErrorNotification()
        {
            await ReInitConnect();
            var notifications = await notificationService.GetErrNotificationsAsync();
            await hubConnection.InvokeAsync(signalRSettings.UpdateErrorNotification, notifications).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    logger.WriteExceptionSentryErrorAsync(task.Exception);
                }
            });
        }

        public async Task PushNewNotification(string notificationIds)
        {
            await ReInitConnect();

            await hubConnection.InvokeAsync(signalRSettings.UpdateNewNotification, notificationIds).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    logger.WriteExceptionSentryErrorAsync(task.Exception);
                }
            });
        }

        public async Task PushNotificationUpdateStatus(string notificationIds)
        {
            await ReInitConnect();

            await hubConnection.InvokeAsync(signalRSettings.UpdateNotificationStatus, notificationIds).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    logger.WriteExceptionSentryErrorAsync(task.Exception);
                }
            });
        }

        private async Task ReInitConnect()
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
        }
    }
}