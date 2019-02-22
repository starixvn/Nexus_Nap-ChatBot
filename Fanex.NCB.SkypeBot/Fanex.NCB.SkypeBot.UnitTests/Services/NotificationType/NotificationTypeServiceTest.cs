using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.NotificationType
{
    public class NotificationTypeServiceTest
    {
        [Fact]
        public async Task GetNotificationTypeListAsync_WhenCalled_CallGetNotificationTypeListAsync()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new NotificationTypeListResultModel {
                Count = 2,
                Data = new List<NotificationTypeItem>() {
                    new NotificationTypeItem()
                }
            };            
            storage.Setup(r => r.GetNotificationTypeListAsync(1,1)).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.GetNotificationTypeListAsync(1,1).ConfigureAwait(false);

            // Assert
            result.Should().Be(fakeData);
        }

        [Fact]
        public async Task GetNotificationSystemsAsync_WhenCalled_CallGetNotificationSystemsAsync()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new List<NotificationSystem> {
                new NotificationSystem()
            };
            storage.Setup(r => r.GetNotificationSystemsAsync()).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.GetNotificationSystemsAsync().ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetRecipientTypesAsync_WhenCalled_CallGetRecipientTypesAsync()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new List<RecipientType>
            {
                new RecipientType()
            };
            storage.Setup(r => r.GetRecipientTypesAsync()).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.GetRecipientTypesAsync().ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task CreateNotificationTypeAsync_WhenCalled_CallCreateNotificationTypeAsync()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new ResultModel();
            storage.Setup(r => r.CreateNotificationTypeAsync(It.IsAny<NotificationTypeItem>())).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.CreateNotificationTypeAsync(null).ConfigureAwait(false);

            // Assert
            result.Should().Be(fakeData);
        }

        [Fact]
        public async Task UpdateNotificationTypeAsync_WhenCalled_CallUpdateNotificationTypeAsync()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new ResultModel();
            storage.Setup(r => r.UpdateNotificationTypeAsync(It.IsAny<NotificationTypeItem>())).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.UpdateNotificationTypeAsync(null).ConfigureAwait(false);

            // Assert
            result.Should().Be(fakeData);
        }

        [Fact]
        public async Task DeleteNotificationTypeAsync_WhenCalled_CallDeleteNotificationTypeAsync()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new ResultModel();
            storage.Setup(r => r.DeleteNotificationTypeAsync(It.IsAny<int>())).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.DeleteNotificationTypeAsync(0).ConfigureAwait(false);

            // Assert
            result.Should().Be(fakeData);
        }
    }
}
