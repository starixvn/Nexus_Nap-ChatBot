namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.Hubs;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using NSubstitute;
    using Xunit;

    public class EditNotificationControllerTests
    {
        private INotificationService notificationService;
        private INapSession napSession;
        private IHubContext<ChatbotHub, IChatbotClient> hubContext;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var id = fixture.Create<int>();

            var actual = controller.Index(id);

            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<int>(viewResult.Model);

            Assert.Equal(id, model);
        }

        [Fact]
        public async Task Edit_ValidModel_ReturnSuccessCommandResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            var viewModel = fixture.Create<EditNotificationViewModel>();
            var model = new EditNotification();

            notificationService.EditAsync(Arg.Do<EditNotification>(x => model = x)).Returns(new CommandResult { Code = 0 });

            var result = await controller.Edit(viewModel);

            var json = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.True(commandResult.IsSuccessful);

            Assert.Equal(viewModel.NotificationId, model.NotificationId);
            Assert.Equal(viewModel.Title, model.Title);
            Assert.Equal(viewModel.Content, model.Content);
            Assert.Equal(viewModel.SendDate, model.SendDate);
            Assert.Equal(viewModel.SendImmediately, model.SendImmediately);
            Assert.Equal(viewModel.RecipientIds, model.RecipientIds);
            Assert.Equal(napSession.UserId, model.UserId);
        }

        [Fact]
        public async Task Edit_InvalidModel_ReturnErrorCommandResult()
        {
            var controller = GetController();
            var fixture = new Fixture();

            var viewModel = fixture.Create<EditNotificationViewModel>();
            viewModel.RecipientIds = string.Empty;
            var result = await controller.Edit(viewModel);
            var json = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.False(commandResult.IsSuccessful);
            Assert.Equal("Invaliad data. The recipients cannot be null", commandResult.Message);
        }

        [Fact]
        public async Task GetRecipients_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();

            var notificationId = fixture.Create<int>();
            var notificationTypeId = fixture.Create<int>();
            var productId = fixture.Create<int?>();
            var platforms = fixture.Create<string>();
            var recipientIds = fixture.Create<string>();
            var integrationMethodIds = fixture.Create<string>();
            var allRecipients = fixture.Create<(IEnumerable<Recipient>, IEnumerable<uint>)>();

            notificationService.GetEditNotificationRecipientsAsync(notificationId, notificationTypeId, productId, platforms, recipientIds, integrationMethodIds).Returns(allRecipients);

            var result = await controller.GetRecipients(notificationId, notificationTypeId, productId, platforms, recipientIds, integrationMethodIds);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var recipients = jsonResult.Value.GetType().GetProperty("recipients").GetValue(jsonResult.Value);
            var selectedRecipients = jsonResult.Value.GetType().GetProperty("selectedRecipients").GetValue(jsonResult.Value);
            Assert.IsAssignableFrom<IEnumerable<TreeNodeItem>>(recipients);
            Assert.Equal(allRecipients.Item2, selectedRecipients);
        }

        [Fact]
        public async Task Get_ValidId_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();

            var notificationId = fixture.Create<int>();

            var notification = fixture.Create<EditNotificationInfo>();

            notificationService.GetEditNotificationInfoAsync(notificationId).Returns(notification);

            var result = await controller.Get(notificationId);

            var json = Assert.IsType<JsonResult>(result);
            var returnedNotification = Assert.IsType<EditNotificationInfo>(json.Value);
            Assert.Equal(notification, returnedNotification);
        }

        [Fact]
        public async Task Get_InvalidId_ReturnNotFoundResult()
        {
            var controller = GetController();

            var notificationId = 0;

            EditNotificationInfo notification = null;

            notificationService.GetEditNotificationInfoAsync(notificationId).Returns(notification);

            var result = await controller.Get(notificationId);

            Assert.IsType<NotFoundResult>(result);
        }

        private EditNotificationController GetController()
        {
            notificationService = Substitute.For<INotificationService>();
            napSession = Substitute.For<INapSession>();
            hubContext = Substitute.For<IHubContext<ChatbotHub, IChatbotClient>>();

            return new EditNotificationController(notificationService, napSession, hubContext);
        }
    }
}