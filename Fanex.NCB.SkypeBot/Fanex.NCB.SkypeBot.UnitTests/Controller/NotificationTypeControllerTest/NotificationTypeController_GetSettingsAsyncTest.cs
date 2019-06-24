using AutoFixture;
using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var controller = new NotificationTypeController(notificationTypeService.Object);

            var recipientTypesResult = fixture.Create<List<RecipientType>>();
            var notificationSystemsResult = fixture.Create<List<NotificationSystem>>();

            notificationTypeService.Setup(s => s.GetRecipientTypesAsync()).Returns(Task.FromResult(recipientTypesResult));
            notificationTypeService.Setup(s => s.GetNotificationSystemsAsync()).Returns(Task.FromResult(notificationSystemsResult));

            // Act
            var result = await controller.GetSettings() as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(new { recipientTypes = recipientTypesResult, notificationSystems = notificationSystemsResult }));
        }
    }
}