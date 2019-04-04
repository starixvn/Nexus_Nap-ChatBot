using System.Data;
using System.Threading.Tasks;
using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.NotificationTypeStorageTest
{
    public class NotificationTypeStorage_CreateNotificationTypeAsyncTest
    {
        [Fact]
        public async Task CreateNotificationTypeAsync_WhenCalled_CallExecuteAsyncWithRightParams()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var item = new CreateNotificationTypeCriteria { NotificationTypeName = "type name", RecipientTypeIds = "2", SystemId = 3 };
            var fakeData = new ResultModel();
            string spKey = null;
            DynamicParameters param = null;
            baseStorage.Setup(r => r.ExecuteAsync(It.IsAny<string>(), It.IsAny<DynamicParameters>(), It.IsAny<string>()))
                .Callback<string, DynamicParameters, string>((m1, m2, m3) => { spKey = m1; param = m2; })
                .Returns(Task.FromResult(fakeData));

            // Act
            await storage.CreateNotificationTypeAsync(item).ConfigureAwait(false);

            // Assert
            DynamicParameters expectedparam = new DynamicParameters();
            expectedparam.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            expectedparam.Add("recipientTypeIds", value: item.RecipientTypeIds, dbType: DbType.String);
            expectedparam.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            spKey.Should().Be("NCB_NotificationType_Ins");
            param.Should().BeEquivalentTo(expectedparam);
        }

        [Fact]
        public async Task CreateNotificationTypeAsync_WhenCalled_ReturnsResultModel()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var item = new CreateNotificationTypeCriteria { NotificationTypeName = "type name", RecipientTypeIds = "2", SystemId = 3 };
            var fakeData = new ResultModel() { ErrorCode = 2, ErrorMsg = "abc"};
            baseStorage.Setup(r => r.ExecuteAsync(It.IsAny<string>(), It.IsAny<DynamicParameters>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeData));

            // Act
            var result = await storage.CreateNotificationTypeAsync(item).ConfigureAwait(false);

            // Assert           
            result.Should().BeEquivalentTo(fakeData);
        }
    }
}
