using AutoFixture;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.ChatGroupMapping
{
    public class ChatGroupMappingService_GetBotNameAsyncTest
    {
        [Fact]
        public async Task GetBotNameAsync_WhenCalled_GetBotNameAsync() {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingStorage = new Mock<IChatGroupMappingStorage>();
            var service = new ChatGroupMappingService(chatGroupMappingStorage.Object);

            var botNames = fixture.CreateMany<BotName>(2).ToList();
            chatGroupMappingStorage.Setup(s => s.GetBotNameAsync()).Returns(Task.FromResult(botNames));

            // Act
            var result = await service.GetBotNameAsync();

            // Assert
            result.Should().BeEquivalentTo(botNames);
        }
    }
}
