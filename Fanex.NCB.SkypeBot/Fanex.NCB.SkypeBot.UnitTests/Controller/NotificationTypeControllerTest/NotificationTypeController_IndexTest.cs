namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
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
            var controller = new NotificationTypeController(notificationTypeService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.ViewName.Should().BeEquivalentTo("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }
    }
}