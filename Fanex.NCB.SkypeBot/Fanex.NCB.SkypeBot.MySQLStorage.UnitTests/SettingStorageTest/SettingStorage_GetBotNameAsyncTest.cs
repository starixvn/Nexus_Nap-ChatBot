using AutoFixture;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.MySQLStorage.Setting;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.SettingStorageTest
{
    public class SettingStorage_GetBotNameAsyncTest
    {
        [Fact]
        public void GetBotNameAsync_WhenCalled_ReturnCorrectData()
        {
            // Arrange
            var fixture = new Fixture();
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new SettingStorage(baseStorage.Object);

            var expectedData = fixture.CreateMany<BotName>().ToList();
            baseStorage.Setup(s => s.QueryAsync<BotName>("NCB_Setting_BotName_Sel", It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(expectedData));

            // Act
            var result = storage.GetBotNameAsync().Result;

            // Assert
            result.Should().BeEquivalentTo(expectedData);
        }
    }
}