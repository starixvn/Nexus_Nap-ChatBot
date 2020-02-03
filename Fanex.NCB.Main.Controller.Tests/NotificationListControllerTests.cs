namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class NotificationListControllerTests
    {
        private INotificationService notificationService;
        private ISignalRService signalRService;
        private INapSession napSession;
        private IUserService userService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();

            var napSettings = Substitute.For<INapSettings>();
            NapClientManager.UseConfig(napSettings);

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotifiactionList, Policy.Host)
                .Returns(true);
            var actual = controller.Index(null, null);

            var viewResult = Assert.IsType<ViewResult>(actual);
            var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveEditNotificationPermission"]);
            Assert.True(haveHostPermission);
        }

        [Fact]
        public void Index_WhenCalledWithParams_ReturnExpectedView()
        {
            // Arrange
            var controller = GetController();
            var napSettings = Substitute.For<INapSettings>();
            NapClientManager.UseConfig(napSettings);
            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();
            var id = fixture.Create<int>();
            var pageIndex = fixture.Create<int>();
            userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotifiactionList, Policy.Host)
                .Returns(true);

            // Action
            var actual = controller.Index(id, pageIndex);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveEditNotificationPermission"]);
            var actualId = Assert.IsType<int>(viewResult.ViewData["HightLightId"]);
            var actualPageIndex = Assert.IsType<int>(viewResult.ViewData["PageIndex"]);
            Assert.True(haveHostPermission);
            Assert.Equal(id, actualId);
            Assert.Equal(pageIndex, actualPageIndex);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var filter = fixture.Create<GetNotificationViewModel>();
            var notifications = fixture.Create<IEnumerable<Notification>>();
            var total = fixture.Create<long>();

            var model = new GetNotifications();

            notificationService.GetNotificationsAsync(Arg.Do<GetNotifications>(x => model = x)).Returns((total, notifications));

            var result = await controller.Get(filter).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(total, (long)jsonResult.Value.GetType().GetProperty("total").GetValue(jsonResult.Value));

            var returnedData = Assert.IsAssignableFrom<IEnumerable<Notification>>(jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value));
            Assert.Equal(notifications, returnedData);
        }

        [Fact]
        public async Task GetNotificationError_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var notificationId = fixture.Create<int>();

            var report = fixture.Create<NotificationErrorReport>();

            notificationService.GetNotificationErrorAsync(notificationId).Returns(report);

            var result = await controller.GetNotificationError(notificationId).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedReport = Assert.IsType<NotificationErrorReport>(jsonResult.Value);

            Assert.Equal(report, returnedReport);
        }

        [Fact]
        public async Task Cancel_WhenCalled_ReturnSuccessCommandResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();
            var notificationId = fixture.Create<int>();

            notificationService.CancelAsync(notificationId, napSession.UserId).Returns(new CommandResult { Code = 0 });

            var result = await controller.Cancel(notificationId).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);

            var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
            SignalRServiceHasBeenCall(notificationId.ToString());
            Assert.True(commandResult.IsSuccessful);
        }

        [Fact]
        public async Task Resolve_WhenCalled_ReturnSuccessCommandResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();
            var data = fixture.Create<ResolveNotificationViewModel>();

            notificationService.ResolveAsync(data.NotificationId, data.ResolvedRemark, napSession.UserId).Returns(new CommandResult { Code = 0 });

            var result = await controller.Resolve(data).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            SignalRServiceHasBeenCall(data.NotificationId.ToString());

            var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        [Fact]
        public async Task GetAllErrorNotifications_WhenCalled_ReturnJsonResult()
        {
            // Arrange
            var controller = GetController();
            var fixture = new Fixture();
            var notificationTypeIds = fixture.Create<string>();
            var recipientIds = fixture.Create<string>();
            var notifications = fixture.Create<IEnumerable<Notification>>();
            var model = new GetErrorNotifications();
            notificationService.GetAllErrorNotificationsAsync(Arg.Do<GetErrorNotifications>(x => model = x)).Returns(notifications);

            // Action
            var result = await controller.GetAllErrorNotifications(notificationTypeIds, recipientIds).ConfigureAwait(false);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(notifications, jsonResult.Value);
        }

        private void SignalRServiceHasBeenCall(string notificationIds)
        {
            Task.Run(() => signalRService.Received().UpdateNotificationStatus(notificationIds));
        }

        private NotificationListController GetController()
        {
            notificationService = Substitute.For<INotificationService>();
            napSession = Substitute.For<INapSession>();
            userService = Substitute.For<IUserService>();
            signalRService = Substitute.For<ISignalRService>();

            return new NotificationListController(notificationService, signalRService, napSession, userService);
        }
    }
}