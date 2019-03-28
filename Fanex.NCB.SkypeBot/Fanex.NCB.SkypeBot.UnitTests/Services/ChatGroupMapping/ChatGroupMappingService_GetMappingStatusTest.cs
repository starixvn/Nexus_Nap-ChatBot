using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.ChatGroupMapping
{
    public class ChatGroupMappingService_GetMappingStatusTest
    {
        [Fact]
        public void GetMappingStatus_WhenCalled_ReturnCorrectData() {
            // Arrange
            var chatGroupMappingStorage = new Mock<IChatGroupMappingStorage>();
            var service = new ChatGroupMappingService(chatGroupMappingStorage.Object);

            var expectedResult = new List<MappingStatus> {
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.NotMapped, Name = "Not Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.Mapped, Name = "Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.BotRemoved, Name = "Bot Removed" }
            };

            // Act
            var result = service.GetMappingStatus();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
