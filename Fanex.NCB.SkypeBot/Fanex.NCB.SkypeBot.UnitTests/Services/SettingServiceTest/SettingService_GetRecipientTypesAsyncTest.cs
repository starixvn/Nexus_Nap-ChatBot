using AutoFixture;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.Setting;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.SettingServiceTest
{
    public class SettingService_GetRecipientTypesAsyncTest
    {
        [Fact]
        public async Task GetRecipientTypesAsync_WhenCalled_GetRecipientTypesAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var settingStorage = new Mock<ISettingStorage>();
            var service = new SettingService(settingStorage.Object);

            var recipientTypes = fixture.CreateMany<RecipientType>(2).ToList();
            settingStorage.Setup(s => s.GetRecipientTypesAsync()).Returns(Task.FromResult(recipientTypes));

            // Act
            var result = await service.GetRecipientTypesAsync();

            // Assert
            result.Should().BeEquivalentTo(recipientTypes);
        }
    }
}
