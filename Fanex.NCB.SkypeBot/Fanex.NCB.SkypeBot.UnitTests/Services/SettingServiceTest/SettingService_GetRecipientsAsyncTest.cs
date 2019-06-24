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
    public class SettingService_GetRecipientsAsyncTest
    {
        [Fact]
        public async Task GetRecipientsAsync_WhenCalled_GetRecipientsAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var settingStorage = new Mock<ISettingStorage>();
            var service = new SettingService(settingStorage.Object);

            var recipients = fixture.CreateMany<Recipient>(2).ToList();
            settingStorage.Setup(s => s.GetRecipientsAsync()).Returns(Task.FromResult(recipients));

            // Act
            var result = await service.GetRecipientsAsync();

            // Assert
            result.Should().BeEquivalentTo(recipients);
        }
    }
}