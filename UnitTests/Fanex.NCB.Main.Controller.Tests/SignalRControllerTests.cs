namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class SignalRControllerTests
    {
        private readonly IOptionsSnapshot<AppSettings> appSettings;
        private readonly ISignalRService signalRService;
        private readonly INotificationService notificationService;
        private readonly SignalRController controller;
        private readonly Fixture fixture;

        public SignalRControllerTests()
        {
            fixture = new Fixture();
            appSettings = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettings.Value.Returns(fixture.Create<AppSettings>());
            signalRService = Substitute.For<ISignalRService>();
            notificationService = Substitute.For<INotificationService>();
            controller = new SignalRController(appSettings, signalRService, notificationService);
        }

        [Fact]
        public void Authenticate_InvalidUsernamPassword_ReturnBadRequestResult()
        {
            var result = controller.Authenticate("test", "a");
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Authenticate_ValidUsernamePassword_ReturnOkResult()
        {
            var result = controller.Authenticate(appSettings.Value.AppCredentialId, appSettings.Value.AppCredentialPassword);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PushSignalOnCreateNotifications_WhenCalled_ExecuteExcpectedService_AndReturnOkResult()
        {
            string notificationIds = fixture.Create<string>();
            var result = await controller.PushSignalOnCreateNotifications(notificationIds);
            await signalRService.Received().InsertNewNotification(notificationIds);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PushSignalUpdateNotificationStatus_WhenCalled_ExecuteExcpectedService_AndReturnOkResult()
        {
            string notificationIds = fixture.Create<string>();
            var result = await controller.PushSignalUpdateNotificationStatus(notificationIds);
            await signalRService.Received().UpdateNotificationStatus(notificationIds);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task HasNewErrNotification_WhenCalled_ExecuteExcpectedService_AndReturnOkResult()
        {
            var notifications = fixture.Create<IEnumerable<NotificationPaging>>();
            notificationService.GetErrNotificationsAsync().Returns(notifications);
            var result = await controller.HasNewErrNotification();
            await signalRService.Received().HasNewErrNotifications(notifications);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}