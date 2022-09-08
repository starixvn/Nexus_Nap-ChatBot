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
    using Fanex.NCB.Main.Hubs;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.NotificationList;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class NotificationListControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private ICommonService commonService;
        private INotificationService notificationService;
        private IPushSignalRService signalRService;
        private INapSession napSession;
        private IUserService userService;
        private IHubContext<ChatbotHub, IChatbotClient> chatHubContext;

        [Fact]
        public async Task GetSettings_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var notificationTypes = new List<NotificationType>
            {
                new NotificationType { NotificationTypeID = 1, NotificationTypeName = "UM" }
            };
            var senders = fixture.Create<IEnumerable<User>>();
            var notificationStatuses = new List<NotificationStatus>
            {
                new NotificationStatus { Id = 1, Name = "Sent" }
            };
            var recipients = new List<Recipient>
            {
                new Recipient
                {
                    RecipientTypeId = 1, RecipientTypeName = "Licensee",
                    LicenseeTypeId = 1, LicenseeTypeName = "Boping API",
                    RecipientId = 1, RecipientName = "67BET"
                },
                new Recipient
                {
                    RecipientTypeId = 2, RecipientTypeName = "3rd Party Product",
                    RecipientId = 2, RecipientName = "AG Casino"
                },
            };
            var channels = new List<ConversationChannel>
            {
                new ConversationChannel { ChannelId = 1, ChannelName = "skype" }
            };
            var controller = GetController();
            commonService.GetNotificationTypesAsync().Returns(notificationTypes);
            commonService.GetUsersAsync().Returns(senders);
            commonService.GetNotificationStatusesAsync().Returns(notificationStatuses);
            commonService.GetRecipientsAsync().Returns(recipients);
            commonService.GetConversationChannelAsync().Returns(channels);

            // Act
            var result = await controller.GetSettings();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var viewModel = Assert.IsType<NotificationListFilterViewModel>(jsonResult.Value);
            Assert.Equal("1", viewModel.NotificationTypeIds);
            Assert.Equal("1", viewModel.NotificationStatusIds);
            Assert.Equal("1,2", viewModel.RecipientIds);
            Assert.Equal("1", viewModel.ChannelIds);
            Assert.Equal(senders, viewModel.Senders);

            Assert.Collection(viewModel.NotificationTypes, item =>
            {
                Assert.Equal(1, item.Id);
                Assert.Equal("UM", item.Text);
                Assert.True(item.Checked);
                Assert.True(item.IsLeaf);
            });

            Assert.Collection(viewModel.NotificationStatuses, item =>
            {
                Assert.Equal(1, item.Id);
                Assert.Equal("Sent", item.Text);
                Assert.True(item.Checked);
                Assert.True(item.IsLeaf);
            });

            Assert.Collection(viewModel.Channels, item =>
            {
                Assert.Equal(1, item.Id);
                Assert.Equal("skype", item.Text);
                Assert.True(item.Checked);
                Assert.True(item.IsLeaf);
            });

            Assert.Collection(viewModel.Recipients,
                item =>
                {
                    Assert.Equal(1, item.Id);
                    Assert.Equal("Licensee", item.Text);
                    Assert.True(item.Checked);
                    Assert.False(item.IsLeaf);

                    Assert.Collection(item.Items, ltype =>
                    {
                        Assert.Equal(1, ltype.Id);
                        Assert.Equal("Boping API", ltype.Text);
                        Assert.True(ltype.Checked);
                        Assert.False(ltype.IsLeaf);

                        Assert.Collection(ltype.Items, r =>
                        {
                            Assert.Equal(1, r.Id);
                            Assert.Equal("67BET", r.Text);
                            Assert.True(r.Checked);
                            Assert.True(r.IsLeaf);
                            Assert.Empty(r.Items);
                        });
                    });
                },
                item =>
                {
                    Assert.Equal(2, item.Id);
                    Assert.Equal("3rd Party Product", item.Text);
                    Assert.True(item.Checked);
                    Assert.False(item.IsLeaf);

                    Assert.Collection(item.Items, ltype =>
                    {
                        Assert.Equal(2, ltype.Id);
                        Assert.Equal("AG Casino", ltype.Text);
                        Assert.True(ltype.Checked);
                        Assert.True(ltype.IsLeaf);
                        Assert.Empty(ltype.Items);
                    });
                });
        }

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();

            var napSettings = Substitute.For<INapSettings>();
            NapClientManager.UseConfig(napSettings);
            napSession.UserId = fixture.Create<int>();

            userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotifiactionList, Policy.Host)
                .Returns(true);
            var actual = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveEditNotificationPermission"]);
            Assert.True(haveHostPermission);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var filter = fixture.Create<GetNotificationViewModel>();
            var notifications = fixture.Create<IEnumerable<Notification>>();
            var total = fixture.Create<long>();

            var model = new GetNotifications();

            notificationService.GetNotificationsAsync(Arg.Do<GetNotifications>(x => model = x)).Returns((total, notifications));

            var result = await controller.Get(filter);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(total, (long)jsonResult.Value.GetType().GetProperty("total").GetValue(jsonResult.Value));

            var returnedData = Assert.IsAssignableFrom<IEnumerable<Notification>>(jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value));
            Assert.Equal(notifications, returnedData);
        }

        [Fact]
        public async Task GetNotificationError_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var notificationId = fixture.Create<int>();

            var report = fixture.Create<NotificationErrorReport>();

            notificationService.GetNotificationErrorAsync(notificationId).Returns(report);

            var result = await controller.GetNotificationError(notificationId);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedReport = Assert.IsType<NotificationErrorReport>(jsonResult.Value);

            Assert.Equal(report, returnedReport);
        }

        [Fact]
        public async Task Cancel_WhenCalled_ReturnSuccessCommandResult()
        {
            var controller = GetController();
            napSession.UserId = fixture.Create<int>();
            var notificationId = fixture.Create<int>();

            notificationService.CancelAsync(notificationId, napSession.UserId).Returns(new CommandResult { Code = 0 });

            var result = await controller.Cancel(notificationId);

            var jsonResult = Assert.IsType<JsonResult>(result);

            var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
            SignalRServiceHasBeenCall(notificationId.ToString());
            Assert.True(commandResult.IsSuccessful);
        }

        [Fact]
        public async Task Resolve_WhenCalled_ReturnSuccessCommandResult()
        {
            var controller = GetController();
            napSession.UserId = fixture.Create<int>();
            var data = fixture.Create<ResolveNotificationViewModel>();

            notificationService.ResolveAsync(data.NotificationId, data.ResolvedRemark, napSession.UserId).Returns(new CommandResult { Code = 0 });

            var result = await controller.Resolve(data);

            var jsonResult = Assert.IsType<JsonResult>(result);
            SignalRServiceHasBeenCall(data.NotificationId.ToString());

            var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        [Fact]
        public async Task GetUpdatedNotification_WhenCalledReturnExpectedJsonResult()
        {
            var controller = GetController();
            var notifictaionIds = fixture.Create<string>();
            var notifications = fixture.Create<IEnumerable<Notification>>();
            notificationService.GetUpdatedNotificationAsync(Arg.Is(notifictaionIds)).Returns(notifications);

            var result = await controller.GetUpdatedNotification(notifictaionIds);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(JsonConvert.SerializeObject(notifications), JsonConvert.SerializeObject(jsonResult.Value));
        }

        [Fact]
        public async Task GetErrNotifications_WhenCalledReturnExpectedJsonResult()
        {
            var controller = GetController();
            var notifications = fixture.Create<IEnumerable<NotificationPaging>>();
            notificationService.GetErrNotificationsAsync().Returns(notifications);

            var result = await controller.GetErrNotifications();

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(JsonConvert.SerializeObject(notifications), JsonConvert.SerializeObject(jsonResult.Value));
        }

        private void SignalRServiceHasBeenCall(string notificationIds)
        {
            Task.Run(() => chatHubContext.Clients.All.Received().UpdateNotificationStatus(notificationIds));
        }

        private NotificationListController GetController()
        {
            commonService = Substitute.For<ICommonService>();
            notificationService = Substitute.For<INotificationService>();
            napSession = Substitute.For<INapSession>();
            userService = Substitute.For<IUserService>();
            signalRService = Substitute.For<IPushSignalRService>();
            chatHubContext = Substitute.For<IHubContext<ChatbotHub, IChatbotClient>>();
            return new NotificationListController(commonService, notificationService, napSession, userService, chatHubContext);
        }
    }
}