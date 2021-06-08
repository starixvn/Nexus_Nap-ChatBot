namespace Fanex.NCB.Main.Controllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class NewNotificationControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private INotificationService notificationService;
        private INapSession napSession;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();

            var actual = controller.Index();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task Create_ValidModel_ReturnSuccessCommandResult()
        {
            var controller = GetController();
            napSession.UserId = fixture.Create<int>();

            var viewModel = new CreateNewNotificationViewModel
            {
                ChannelIds = "1,2",
                NotificationTypeId = 1,
                ProductId = 1,
                RecipientIds = "1,2",
                Title = "title",
                Content = "**content**",
                SendDate = DateTime.Now,
                SendImmediately = true
            };
            var contents = new List<NotificationTemplateContent>
            {
                new NotificationTemplateContent { ChannelId = 1, Content = "**content**" },
                new NotificationTemplateContent { ChannelId = 2, Content = "*content*" }
            };
            var model = new CreateNotification();

            notificationService.CreateAsync(Arg.Do<CreateNotification>(x => model = x)).Returns(new CommandResult { Code = 0 });

            var result = await controller.Create(viewModel);

            var json = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.True(commandResult.IsSuccessful);

            Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeId);
            Assert.Equal(viewModel.ProductId, model.ProductId);
            Assert.Equal(viewModel.Title, model.Title);
            Assert.Equal(JsonConvert.SerializeObject(contents), model.Content);
            Assert.Equal(viewModel.SendDate, model.SendDate);
            Assert.Equal(viewModel.SendImmediately, model.SendImmediately);
            Assert.Equal(viewModel.RecipientIds, model.RecipientIds);
            Assert.Equal(napSession.UserId, model.CreatorId);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnErrorCommandResult()
        {
            var controller = GetController();
            napSession.UserId = fixture.Create<int>();

            var viewModel = fixture.Create<CreateNewNotificationViewModel>();
            viewModel.RecipientIds = string.Empty;
            var result = await controller.Create(viewModel);
            var json = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.False(commandResult.IsSuccessful);
            Assert.Equal("Invaliad data. The recipients cannot be null", commandResult.Message);
        }

        [Fact]
        public async Task GetRecipients_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var notificationTypeId = fixture.Create<int>();
            var productId = fixture.Create<int?>();

            var recipients = fixture.Create<IEnumerable<Recipient>>();

            notificationService.GetRecipientsAsync(notificationTypeId, productId).Returns(recipients);

            var result = await controller.GetRecipients(notificationTypeId, productId);

            var json = Assert.IsType<JsonResult>(result);
            Assert.IsAssignableFrom<IEnumerable<TreeNodeItem>>(json.Value);
        }

        [Fact]
        public async Task GetTemplates_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var notificationTypeId = fixture.Create<int>();

            var templates = fixture.Create<IEnumerable<Template>>();

            notificationService.GetTemplatesAsync(notificationTypeId).Returns(templates);

            var result = await controller.GetTemplates(notificationTypeId);

            var json = Assert.IsType<JsonResult>(result);
            var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<Template>>(json.Value);
            Assert.Equal(templates, returnedTemplates);
        }

        [Fact]
        public async Task GetResendNotification_WhenCalled_ReturnExpectedJsonResult()
        {
            var controller = GetController();
            var notificationId = fixture.Create<int>();
            var isResendAll = fixture.Create<bool>();
            var resendNotifictaion = fixture.Create<ResendNotification>();
            var allRecipients = fixture.Create<IEnumerable<Recipient>>();
            var unSelectable = resendNotifictaion.Recipients.Where(r => !allRecipients.Any(x => x.RecipientId == r.RecipientId));

            var expected = new
            {
                resendNotification = resendNotifictaion,
                recipients = GenerateTreeHelper.FromRecipient(allRecipients),
                unSelectableRecipients = unSelectable
            };
            notificationService.GetResendNotificationInfoAsync(notificationId, isResendAll).Returns(resendNotifictaion);
            notificationService.GetRecipientsAsync(resendNotifictaion.NotificationTypeId, null, resendNotifictaion.ChannelId.ToString()).Returns(allRecipients);

            var result = await controller.GetResendNotification(notificationId, isResendAll);

            var json = Assert.IsType<JsonResult>(result);
            var obj = Assert.IsAssignableFrom<object>(json.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(obj));
        }

        private NewNotificationController GetController()
        {
            notificationService = Substitute.For<INotificationService>();
            napSession = Substitute.For<INapSession>();

            return new NewNotificationController(notificationService, napSession);
        }
    }
}