using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Fanex.NCB.Core.Models;
using Fanex.NCB.Models.NotificationType;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_GetSettingsAsyncTest
    {
        [Fact]
        public async Task GetSettingsAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var napSession = new Mock<INapSession>();
            var napSettings = new Mock<INapSettings>();
            var userService = new Mock<IUserService>();
            var controller = new NotificationTypeController(notificationTypeService.Object, napSession.Object, napSettings.Object, userService.Object);

            var recipientTypesResult = fixture.Create<IEnumerable<NotificationRecipientType>>();
            var notificationSystemsResult = fixture.Create<IEnumerable<BaseModel>>();

            notificationTypeService.Setup(s => s.GetRecipientType()).Returns(Task.FromResult(recipientTypesResult));
            notificationTypeService.Setup(s => s.GetSystem()).Returns(Task.FromResult(notificationSystemsResult));

            // Act
            var result = await controller.GetSettings() as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(new { recipientTypes = recipientTypesResult, notificationSystems = notificationSystemsResult }));
        }
    }
}