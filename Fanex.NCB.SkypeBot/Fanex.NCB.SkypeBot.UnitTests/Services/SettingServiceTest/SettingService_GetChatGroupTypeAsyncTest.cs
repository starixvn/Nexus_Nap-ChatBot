using AutoFixture;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.Setting;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.SettingServiceTest
{
    public class SettingService_GetChatGroupTypeAsyncTest
    {
        [Fact]
        public async Task GetChatGroupTypeAsync_WhenCalled_GetChatGroupTypeAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var settingStorage = new Mock<ISettingStorage>();
            var service = new SettingService(settingStorage.Object);

            var chatGroupTypes = fixture.CreateMany<ChatGroupType>(2).ToList();
            settingStorage.Setup(s => s.GetChatGroupTypeAsync()).Returns(Task.FromResult(chatGroupTypes));

            // Act
            var result = await service.GetChatGroupTypeAsync();

            // Assert
            result.Should().BeEquivalentTo(chatGroupTypes);
        }
    }
}