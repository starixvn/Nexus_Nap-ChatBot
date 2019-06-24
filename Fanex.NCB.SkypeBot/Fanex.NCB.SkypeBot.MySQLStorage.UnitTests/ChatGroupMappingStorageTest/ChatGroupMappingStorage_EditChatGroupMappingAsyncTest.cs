using AutoFixture;
using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.MySQLStorage.ChatGroupMapping;
using FluentAssertions;
using Moq;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.ChatGroupMappingStorageTest
{
    public class ChatGroupMappingStorage_EditChatGroupMappingAsyncTest
    {
        [Fact]
        public void EditChatGroupMappingAsync_WhenCalled_CallExecuteAsyncWithRightParams()
        {
            // Arrange
            var fixture = new Fixture();
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new ChatGroupMappingStorage(baseStorage.Object);

            var criteria = fixture.Create<EditChatGroupMappingCriteria>();
            string spName = null;
            DynamicParameters param = null;
            baseStorage.Setup(s => s.ExecuteAsync(It.IsAny<string>(), It.IsAny<DynamicParameters>(), It.IsAny<string>()))
                .Callback<string, DynamicParameters, string>((p1, p2, p3) => { spName = p1; param = p2; });

            DynamicParameters expectedParam = new DynamicParameters();
            expectedParam.Add("groupID", value: criteria.GroupID, dbType: DbType.Int32);
            expectedParam.Add("recipientIDs", value: criteria.RecipientIDs, dbType: DbType.String);
            expectedParam.Add("recipientTypeID", value: criteria.RecipientTypeID, dbType: DbType.Int32);
            expectedParam.Add("creatorID", value: criteria.CreatorID, dbType: DbType.Int32);
            expectedParam.Add("chatGroupTypeId", value: criteria.ChatGroupTypeId, dbType: DbType.Int32);
            expectedParam.Add("remark", value: criteria.Remark, dbType: DbType.String);

            // Act
            storage.EditChatGroupMappingAsync(criteria);

            // Assert
            spName.Should().Be("NCB_ChatGroupMapping_Upd");
            param.Should().BeEquivalentTo(expectedParam);
        }

        [Fact]
        public void EditChatGroupMappingAsync_WhenCalled_ReturnCorrectData()
        {
            // Arrange
            var fixture = new Fixture();
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new ChatGroupMappingStorage(baseStorage.Object);

            var criteria = fixture.Create<EditChatGroupMappingCriteria>();
            var resultModel = fixture.Create<ResultModel>();
            baseStorage.Setup(s => s.ExecuteAsync(It.IsAny<string>(), It.IsAny<DynamicParameters>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultModel));

            // Act
            var result = storage.EditChatGroupMappingAsync(criteria).Result;

            // Assert
            result.Should().BeEquivalentTo(resultModel);
        }
    }
}