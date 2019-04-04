using AutoFixture;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.ChatGroupMappingServiceTest
{
    public class ChatGroupMappingService_UpdateChatGroupMappingAsyncTest
    {
        [Fact]
        public async Task UpdateChatGroupMappingAsync_WhenCalled_EditChatGroupMappingAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingStorage = new Mock<IChatGroupMappingStorage>();
            var chatGroupMappingResultService = new Mock<IChatGroupMappingResultService>();
            var service = new ChatGroupMappingService(chatGroupMappingStorage.Object, chatGroupMappingResultService.Object);

            var criteria = fixture.Create<EditChatGroupMappingCriteria>();
            var resultModel = fixture.Create<ResultModel>();
            chatGroupMappingStorage.Setup(s => s.EditChatGroupMappingAsync(criteria)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await service.UpdateChatGroupMappingAsync(criteria);

            // Assert
            result.Should().BeEquivalentTo(resultModel);
        }
    }
}
