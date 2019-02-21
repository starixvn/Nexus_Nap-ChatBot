using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.NotificationType
{
    public class NotificationTypeStorage_GetNotificationTypeListAsyncTest
    {
        [Fact]
        public async Task GetNotificationTypeListAsync_WhenCalled_CallQueryMultipleAsyncWithRightParams()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var pageIndex = 1;
            var pageSize = 5;
            var fakeData = Tuple.Create(1, new List<NotificationTypeItem>() { new NotificationTypeItem { } });            

            string spKey = null;
            object param = null;            
            baseStorage.Setup(r => r.QueryMultipleAsync<int, NotificationTypeItem>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Callback<string, object, string>((m1, m2, m3) => { spKey = m1; param = m2; })
                .Returns(Task.FromResult(fakeData));

            // Act
           await storage.GetNotificationTypeListAsync(pageIndex, pageSize).ConfigureAwait(false);

            // Assert
            var expectedParam = new { pageIndex, pageSize };
            spKey.Should().Be("ChatBot_NotificationType_Sel");
            param.Should().BeEquivalentTo(expectedParam);
        }

        [Fact]
        public async Task GetNotificationTypeListAsync_WhenCalled_ReturnsNotificationTypeListResultModel()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var fakeData = Tuple.Create(1, new List<NotificationTypeItem>() { new NotificationTypeItem { } });
            var expectedResult = new NotificationTypeListResultModel { Count = fakeData.Item1, Data = fakeData.Item2 };

            baseStorage.Setup(r => r.QueryMultipleAsync<int, NotificationTypeItem>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeData));

            // Act
            var result = await storage.GetNotificationTypeListAsync(1, 1).ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
