using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.NotificationType
{
    public class NotificationTypeStorage_DeleteNotificationTypeAsyncTest
    {
        [Fact]
        public async Task DeleteNotificationTypeAsync_WhenCalled_CallExecuteAsyncWithRightParams()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var notificationTypeId = 2;
            var fakeData = new ResultModel();
            string spKey = null;
            DynamicParameters param = null;
            baseStorage.Setup(r => r.ExecuteAsync(It.IsAny<string>(), It.IsAny<DynamicParameters>(), It.IsAny<string>()))
                .Callback<string, DynamicParameters, string>((m1, m2, m3) => { spKey = m1; param = m2; })
                .Returns(Task.FromResult(fakeData));

            // Act
            await storage.DeleteNotificationTypeAsync(notificationTypeId).ConfigureAwait(false);

            // Assert
            DynamicParameters expectedparam = new DynamicParameters();
            expectedparam.Add("notificationTypeId", value: notificationTypeId, dbType: DbType.Int32);

            spKey.Should().Be("ChatBot_NotificationType_Del");
            param.Should().BeEquivalentTo(expectedparam);
        }

        [Fact]
        public async Task DeleteNotificationTypeAsync_WhenCalled_ReturnsResultModel()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var notificationTypeId = 2;
            var fakeData = new ResultModel() { ErrorCode = 2, ErrorMsg = "abc" };
            baseStorage.Setup(r => r.ExecuteAsync(It.IsAny<string>(), It.IsAny<DynamicParameters>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeData));

            // Act
            var result = await storage.DeleteNotificationTypeAsync(notificationTypeId).ConfigureAwait(false);

            // Assert           
            result.Should().BeEquivalentTo(fakeData);
        }
    }
}
