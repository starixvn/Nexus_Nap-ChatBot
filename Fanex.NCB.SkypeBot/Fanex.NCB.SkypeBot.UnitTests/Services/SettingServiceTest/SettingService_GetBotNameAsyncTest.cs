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
    public class SettingService_GetBotNameAsyncTest
    {
        [Fact]
        public async Task GetBotNameAsync_WhenCalled_GetBotNameAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var settingStorage = new Mock<ISettingStorage>();
            var service = new SettingService(settingStorage.Object);

            var botNames = fixture.CreateMany<BotName>(2).ToList();
            settingStorage.Setup(s => s.GetBotNameAsync()).Returns(Task.FromResult(botNames));

            // Act
            var result = await service.GetBotNameAsync();

            // Assert
            result.Should().BeEquivalentTo(botNames);
        }
    }
}