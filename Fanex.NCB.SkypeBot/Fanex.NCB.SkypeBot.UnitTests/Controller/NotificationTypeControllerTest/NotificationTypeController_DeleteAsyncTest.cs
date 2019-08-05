using System.Threading.Tasks;
using AutoFixture;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_DeleteAsyncTest
    {
        [Fact]
        public async Task DeleteAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var napSession = new Mock<INapSession>();
            var napSettings = new Mock<INapSettings>();
            var userService = new Mock<IUserService>();
            var controller = new NotificationTypeController(notificationTypeService.Object, napSession.Object, napSettings.Object, userService.Object);

            var notificationTypeId = fixture.Create<int>();
            var resultModel = fixture.Create<CommandResult>();
            notificationTypeService.Setup(s => s.DeleteAsync(notificationTypeId)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.Delete(notificationTypeId) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}