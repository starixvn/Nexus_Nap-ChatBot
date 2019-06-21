using AutoFixture;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_EditAsyncTest
    {
        [Fact]
        public async Task EditAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var controller = new NotificationTypeController(notificationTypeService.Object);

            var criteria = fixture.Create<CreateNotificationTypeCriteria>();
            var resultModel = fixture.Create<ResultModel>();
            notificationTypeService.Setup(s => s.UpdateNotificationTypeAsync(criteria)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.Edit(criteria) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}