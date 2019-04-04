using AutoFixture;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.MySQLStorage.Setting;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.SettingStorageTest
{
    public class SettingStorage_GetRecipientsAsyncTest
    {
        [Fact]
        public void GetRecipientsAsync_WhenCalled_ReturnCorrectData()
        {
            // Arrange
            var fixture = new Fixture();
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new SettingStorage(baseStorage.Object);

            var expectedData = fixture.CreateMany<Recipient>().ToList();
            baseStorage.Setup(s => s.QueryAsync<Recipient>("NCB_Recipient_Sel", It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(expectedData));

            // Act
            var result = storage.GetRecipientsAsync().Result;

            // Assert
            result.Should().BeEquivalentTo(expectedData);
        }
    }
}
