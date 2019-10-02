namespace Fanex.NCB.Main.Controllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.NotificationTemplate;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class NotificationTemplateControllerTests
    {
        private INotificationTemplaceService notificationTemplaceService;
        private INapSession napSession;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();

            var napSettings = Substitute.For<INapSettings>();
            var userService = Substitute.For<IUserService>();
            NapClientManager.UseConfig(napSettings);

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotificationTemplate, Policy.Host).Returns(true);
            var actual = controller.Index(userService);

            var viewResult = Assert.IsType<ViewResult>(actual);
            var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveHostPermission"]);
            Assert.True(haveHostPermission);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var notificationTypeId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var total = fixture.Create<long>();
            var templates = fixture.Create<IEnumerable<NotificationTemplate>>();

            notificationTemplaceService.GetAsync(notificationTypeId, pageIndex, pageSize).Returns((total, templates));

            var result = await controller.Get(notificationTypeId, pageIndex, pageSize).ConfigureAwait(false);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var templateId = fixture.Create<int>();

            var template = fixture.Create<NotificationTemplate>();

            notificationTemplaceService.GetByIdAsync(templateId).Returns(template);

            var result = await controller.GetById(templateId).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var returnedTemplate = Assert.IsType<NotificationTemplate>(jsonResult.Value);
            Assert.Equal(template, returnedTemplate);
        }

        [Fact]
        public async Task Create_ValidModel_ReturnSuccessCommandResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            var viewModel = fixture.Create<CreateNotificationTemplateViewModel>();
            var model = new CreateNotificationTemplate();

            notificationTemplaceService.CreateAsync(Arg.Do<CreateNotificationTemplate>(x => model = x)).Returns(new CommandResult { Code = 0 });

            var result = await controller.Create(viewModel).ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.True(commandResult.IsSuccessful);

            Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeId);
            Assert.Equal(viewModel.NotificationTemplateName, model.NotificationTemplateName);
            Assert.Equal(viewModel.Title, model.Title);
            Assert.Equal(viewModel.Content, model.Content);
            Assert.Equal(napSession.UserId, model.CreatorId);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnSuccessCommandResult()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("NotificationTemplateName", "required");

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            var viewModel = fixture.Create<CreateNotificationTemplateViewModel>();

            var result = await controller.Create(viewModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Edit_ValidModel_ReturnSuccessCommandResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            var viewModel = fixture.Create<EditNotificationTemplateViewModel>();
            var model = new EditNotificationTemplate();

            notificationTemplaceService.EditAsync(Arg.Do<EditNotificationTemplate>(x => model = x)).Returns(new CommandResult { Code = 0 });

            var result = await controller.Edit(viewModel).ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var commandResult = Assert.IsType<CommandResult>(json.Value);
            Assert.True(commandResult.IsSuccessful);

            Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeId);
            Assert.Equal(viewModel.NotificationTemplateName, model.NotificationTemplateName);
            Assert.Equal(viewModel.Title, model.Title);
            Assert.Equal(viewModel.Content, model.Content);
            Assert.Equal(napSession.UserId, model.ModifierId);
        }

        [Fact]
        public async Task Edit_InvalidModel_ReturnSuccessCommandResult()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("NotificationTemplateName", "required");

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();

            var viewModel = fixture.Create<EditNotificationTemplateViewModel>();

            var result = await controller.Edit(viewModel).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnSuccessCommandResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            napSession.UserId = fixture.Create<int>();
            var templateId = fixture.Create<int>();

            notificationTemplaceService.DeleteAsync(templateId, napSession.UserId).Returns(new CommandResult { Code = 0 });

            var result = await controller.Delete(templateId).ConfigureAwait(false);

            var jsonResult = Assert.IsType<JsonResult>(result);

            var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
        }

        private NotificationTemplateController GetController()
        {
            notificationTemplaceService = Substitute.For<INotificationTemplaceService>();
            napSession = Substitute.For<INapSession>();

            return new NotificationTemplateController(notificationTemplaceService, napSession);
        }
    }
}