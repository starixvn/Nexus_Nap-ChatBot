namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Models.NotificationType;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.SkypeBot.Controllers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class NotificationTypeController_GetAllAsyncTest
    {
        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var controller = new NotificationTypeController(notificationTypeService.Object);

            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var type = fixture.Create<IEnumerable<NotificationType>>();
            var resultModel = new ValueTuple<IEnumerable<NotificationType>, long>(type, 10);
            notificationTypeService.Setup(s => s.GetAllAsync(pageIndex, pageSize)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.GetAll(pageIndex, pageSize) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}