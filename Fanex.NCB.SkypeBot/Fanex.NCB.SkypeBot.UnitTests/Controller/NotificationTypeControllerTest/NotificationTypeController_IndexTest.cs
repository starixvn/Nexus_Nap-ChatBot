namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.SkypeBot.Controllers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class NotificationTypeController_IndexPageAsyncTest
    {
        [Fact]
        public void IndexPageAsync_WhenCalled_ReturnViewCorrectly()
        {
            // Arrange
            var notificationTypeService = new Mock<INotificationTypeService>();
            var napSession = new Mock<INapSession>();
            var napSettings = new Mock<INapSettings>();
            var userService = new Mock<IUserService>();
            var controller = new NotificationTypeController(notificationTypeService.Object, napSession.Object, napSettings.Object, userService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.ViewName.Should().BeEquivalentTo("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }
    }
}