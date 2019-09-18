using AutoFixture;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Fanex.NCB.Core.Constants;
using Fanex.NCB.Core.Models;
using Fanex.NCB.Models.NotificationType;
using Fanex.NCB.Models.Setting;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Fanex.NAP.Core.Business.Constant;

namespace Fanex.NCB.Main.Controllers.Tests
{
    public class NotificationTypeControllerTest
    {
        private readonly Fixture fixture;        
        private readonly INotificationTypeService notificationTypeService;
        private readonly ISettingService settingService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly IUserService userService;
        private readonly NotificationTypeController controller;

        public NotificationTypeControllerTest()
        {
            fixture = new Fixture();
            notificationTypeService = Substitute.For<INotificationTypeService>();
            settingService = Substitute.For<ISettingService>();            
            userService = Substitute.For<IUserService>();
            napSession = Substitute.For<INapSession>();
            napSettings = Substitute.For<INapSettings>();
            controller = new NotificationTypeController(notificationTypeService, settingService, napSession, napSettings, userService);            
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnNotificationTypeIndexView()
        {   
            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            result.ViewName.Should().Be("~/Views/SkypeBot/NotificationType/Index.cshtml");            
        }

        [Theory]
        [InlineData(Policy.Host, true)]
        [InlineData(Policy.Read, false)]
        public async Task Index_WhenCalled_ReturnHasHostPermissionAsModel(Policy policy, bool expectedResult)
        {
            // Arrange
            var userId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            napSession.UserId.Returns(userId);
            napSettings.SiteId.Returns(siteId);
            userService.GetPermissionAsync(userId, siteId, FunctionKey.ChatGroupMapping).Returns((int)policy);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            result.Model.Should().Be(expectedResult);
        }
      
        [Fact]
        public async Task GetAll_WhenCalled_GetAllNotificationTypes()
        {
            // Arrange
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var notificationTypes = fixture.CreateMany<NotificationType>();
            var count = fixture.Create<long>();
            var data = new ValueTuple<IEnumerable<NotificationType>, long>(notificationTypes, count);
            notificationTypeService.GetAllAsync(pageIndex, pageSize).Returns(data);

            // Act
            var result = await controller.GetAll(pageIndex, pageSize) as JsonResult;

            // Assert
            var expectedResult = new { NotificationTypes = data.Item1, Count = data.Item2 };
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Delete_WhenCalled_DeleteAsync()
        {
            // Arrange
            var notificationTypeId = fixture.Create<int>();
            var userId = fixture.Create<int>();
            napSession.UserId.Returns(userId);
            var commandResult = fixture.Create<CommandResult>();
            notificationTypeService.DeleteAsync(notificationTypeId, userId).Returns(commandResult);

            // Act
            var result = await controller.Delete(notificationTypeId) as JsonResult;

            // Assert
            result.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task Add_WhenCalled_InsertAsync()
        {
            // Arrange
            var model = fixture.Create<NotificationTypeViewModel>();
            var userId = fixture.Create<int>();
            napSession.UserId.Returns(userId);
            var commandResult = fixture.Create<CommandResult>();
            notificationTypeService.InsertAsync(Arg.Do<NotificationTypeViewModel>(x => model = x)).Returns(commandResult);

            // Act
            var result = await controller.Add(model) as JsonResult;

            // Assert
            model.UserId.Should().Be(userId);
            result.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task Edit_WhenCalled_UpdateAsync()
        {
            // Arrange
            var model = fixture.Create<NotificationTypeViewModel>();
            var userId = fixture.Create<int>();
            napSession.UserId.Returns(userId);
            var commandResult = fixture.Create<CommandResult>();
            notificationTypeService.UpdateAsync(Arg.Do<NotificationTypeViewModel>(x => model = x)).Returns(commandResult);

            // Act
            var result = await controller.Edit(model) as JsonResult;

            // Assert
            model.UserId.Should().Be(userId);
            result.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task GetSettings_WhenCalled_GetSettings()
        {
            // Arrange
            var recipientTypes = fixture.CreateMany<BaseModel>();
            var systems = fixture.CreateMany<BaseModel>();
            var chatGroupTypes = fixture.CreateMany<ChatGroupType>();
            settingService.GetRecipientTypesAsync().Returns(recipientTypes);
            settingService.GetSystemAsync().Returns(systems);
            settingService.GetChatGroupTypeAsync().Returns(chatGroupTypes);

            // Act
            var result = await controller.GetSettings() as JsonResult;

            // Assert
            var expectedResult = new { recipientTypes = recipientTypes, notificationSystems = systems, chatGroupTypes = chatGroupTypes };
            result.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}
