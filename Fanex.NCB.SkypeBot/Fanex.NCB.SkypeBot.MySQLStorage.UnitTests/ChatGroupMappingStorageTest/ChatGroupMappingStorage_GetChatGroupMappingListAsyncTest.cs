using AutoFixture;
using Dapper;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.MySQLStorage.ChatGroupMapping;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.ChatGroupMappingStorageTest
{
    public class ChatGroupMappingStorage_GetChatGroupMappingListAsyncTest
    {
        [Fact]
        public void GetChatGroupMappingListAsync_WhenCalled_CallExecuteAsyncWithRightParams()
        {
            // Arrange
            var fixture = new Fixture();
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new ChatGroupMappingStorage(baseStorage.Object);

            var criteria = fixture.Create<GetChapGroupMappingCriteria>();
            string spName = null;
            object param = null;
            baseStorage.Setup(s => s.QueryMultipleAsync<long, BotConversation, BotConversationMember, ConversationRecipient>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Callback<string, object, string>((p1, p2, p3) => { spName = p1; param = p2; });

            DynamicParameters expectedParam = new DynamicParameters();
            expectedParam.Add("botNames", value: criteria.BotNames, dbType: DbType.String);
            expectedParam.Add("recipientTypeIDs", value: criteria.RecipientTypeIDs, dbType: DbType.String);
            expectedParam.Add("recipientName", value: criteria.RecipientName, dbType: DbType.String);
            expectedParam.Add("mappingStatusIds", value: criteria.MappingStatusIds, dbType: DbType.String);
            expectedParam.Add("licenseeStatusIds", value: criteria.LicenseeStatusIds, dbType: DbType.String);
            expectedParam.Add("pageIndex", value: criteria.PageIndex, dbType: DbType.Int64);
            expectedParam.Add("pageSize", value: criteria.PageSize, dbType: DbType.Int64);

            // Act
            storage.GetChatGroupMappingListAsync(criteria);

            // Assert
            spName.Should().Be("NCB_ChatGroupMapping_Sel");
            param.Should().BeEquivalentTo(expectedParam);
        }

        [Fact]
        public void GetChatGroupMappingListAsync_WhenCalled_ReturnCorrectData()
        {
            // Arrange
            var fixture = new Fixture();
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new ChatGroupMappingStorage(baseStorage.Object);

            var criteria = fixture.Create<GetChapGroupMappingCriteria>();
            var count = fixture.Create<int>();
            var botConversationList = fixture.CreateMany<BotConversation>().ToList();
            var botConversationMemberList = fixture.CreateMany<BotConversationMember>().ToList();
            var conversationRecipientList = fixture.CreateMany<ConversationRecipient>().ToList();
            var expectedData = Tuple.Create<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>(count, botConversationList, botConversationMemberList, conversationRecipientList);

            baseStorage.Setup(s => s.QueryMultipleAsync<long, BotConversation, BotConversationMember, ConversationRecipient>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(expectedData));

            // Act
            var result = storage.GetChatGroupMappingListAsync(criteria).Result;

            // Assert
            result.Should().BeEquivalentTo(expectedData);
        }
    }
}
