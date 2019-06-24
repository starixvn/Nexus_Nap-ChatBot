using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.NotificationTypeStorageTest
{
    public class NotificationTypeStorage_GetNotificationSystemsAsyncTest
    {
        [Fact]
        public async Task GetNotificationSystemsAsync_WhenCalled_CallQueryAsyncWithRightParams()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var fakeData = new List<NotificationSystem>() { new NotificationSystem() };
            string spKey = null;
            baseStorage.Setup(r => r.QueryAsync<NotificationSystem>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Callback<string, object, string>((m1, _, __) => spKey = m1)
                .Returns(Task.FromResult(fakeData));

            // Act
            await storage.GetNotificationSystemsAsync().ConfigureAwait(false);

            // Assert
            spKey.Should().Be("NCB_System_Sel");
        }

        [Fact]
        public async Task GetNotificationSystemsAsync_WhenCalled_ReturnsListOfNotificationSystem()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var fakeData = new List<NotificationSystem>() { new NotificationSystem(), new NotificationSystem() };
            baseStorage.Setup(r => r.QueryAsync<NotificationSystem>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeData));

            // Act
            var result = await storage.GetNotificationSystemsAsync().ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(fakeData);
        }
    }
}