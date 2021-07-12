namespace Fanex.NCB.Main.Controllers.Tests
{
    using AutoFixture;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Models.ViewLog;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class ViewLogControllerTests
    {
        private IViewLogService viewLogService;
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

            userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.ViewLog, Policy.Host).Returns(true);
            var actual = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveHostPermission"]);
            Assert.True(haveHostPermission);
        }

        [Fact]
        public async Task GetSystemMessage_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var notificationTypeId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var total = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<ViewLogNotificationSystemMessage>>();

            viewLogService.GetNotificationSystemMessage(notificationTypeId, pageIndex, pageSize).Returns((total, templates));

            var result = await controller.GetSystemMessage(notificationTypeId, pageIndex, pageSize);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetRecipientList_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var notificationTypeId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var total = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<ViewLogNotificationRecipientList>>();

            viewLogService.GetNotificationRecipientList(notificationTypeId, pageIndex, pageSize).Returns((total, templates));

            var result = await controller.GetRecipientList(notificationTypeId, pageIndex, pageSize);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetConversationMessage_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();

            var fixture = new Fixture();
            var notificationTypeId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var total = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<ViewLogConversationMessageLog>>();

            viewLogService.GetConversationMessageLog(notificationTypeId, pageIndex, pageSize).Returns((total, templates));

            var result = await controller.GetConversationMessage(notificationTypeId, pageIndex, pageSize);

            Assert.IsType<JsonResult>(result);
        }

        private ViewLogController GetController()
        {
            viewLogService = Substitute.For<IViewLogService>();
            napSession = Substitute.For<INapSession>();
            userService = Substitute.For<IUserService>();

            return new ViewLogController(viewLogService, napSession, userService);
        }
    }
}
