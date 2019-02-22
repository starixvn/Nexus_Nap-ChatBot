using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.MySQLStorage.UnitTests.NotificationType
{
    public class NotificationTypeStorage_GetRecipientTypesAsyncTest
    {
        [Fact]
        public async Task GetRecipientTypesAsync_WhenCalled_CallQueryAsyncWithRightParams()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var fakeData = new List<RecipientType>() { new RecipientType() };
            string spKey = null;
            baseStorage.Setup(r => r.QueryAsync<RecipientType>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Callback<string, object, string>((m1, __, _) => spKey = m1)
                .Returns(Task.FromResult(fakeData));

            // Act
            await storage.GetRecipientTypesAsync().ConfigureAwait(false);

            // Assert
            spKey.Should().Be("ChatBot_RecipientType_Sel");
        }

        [Fact]
        public async Task GetRecipientTypesAsync_WhenCalled_ReturnsListOfRecipientType()
        {
            // Arrange
            var baseStorage = new Mock<IBaseStorage>();
            var storage = new NotificationTypeStorage(baseStorage.Object);

            var fakeData = new List<RecipientType>() { new RecipientType(), new RecipientType() };
            baseStorage.Setup(r => r.QueryAsync<RecipientType>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeData));

            // Act
            var result = await storage.GetRecipientTypesAsync().ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(fakeData);
        }
    }
}
