namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Notification;
    using Microsoft.AspNetCore.SignalR;
    using NSubstitute;
    using Xunit;

    public class SignalRServiceTests
    {
        private readonly IHubContext<SignalRService> hubContext;
        private readonly SignalRService service;

        public SignalRServiceTests()
        {
            hubContext = Substitute.For<IHubContext<SignalRService>>();
            service = new SignalRService(hubContext);
        }

        [Fact]
        public async Task InsertNewNotification_WhenEmptyNotificationIds_NotCallsHubContextClient()
        {
            await service.InsertNewNotification(string.Empty);

            hubContext.DidNotReceive();
        }

        [Fact]
        public async Task InsertNewNotification_WhenNotEmptyNotificationIds_CallsHubContextClient()
        {
            await service.InsertNewNotification("1,2,3");

            hubContext.ReceivedCalls();
        }

        [Fact]
        public async Task UpdateNotificationStatus_WhenEmptyNotificationIds_NotCallsHubContextClient()
        {
            await service.UpdateNotificationStatus(string.Empty);

            hubContext.DidNotReceive();
        }

        [Fact]
        public async Task UpdateNotificationStatus_WhenNotEmptyNotificationIds_CallsHubContextClient()
        {
            await service.UpdateNotificationStatus("1,2,3");

            hubContext.ReceivedCalls();
        }

        [Fact]
        public async Task HasNewErrNotifications_WhenEmptyListNotification_NotCallsHubContextClient()
        {
            await service.HasNewErrNotifications(new List<Notification>());

            hubContext.DidNotReceive();
        }

        [Fact]
        public async Task HasNewErrNotifications_WhenNotEmptyListNotification_CallsHubContextClient()
        {
            await service.HasNewErrNotifications(new List<Notification> { new Notification { NotificationId = 1 } });

            hubContext.ReceivedCalls();
        }
    }
}