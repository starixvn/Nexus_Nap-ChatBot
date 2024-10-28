namespace Fanex.NCB.Criteria.Tests.ViewLog
{
    using Fanex.NCB.AutoJobs.Core.Criteria.Common;
    using FluentAssertions;
    using Xunit;

    public class DeleteOldConversationMessageLogCommandTest
    {
        [Fact]
        public void DeleteNotificationTypeCommand_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new DeleteOldConversationMessageLogCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_ViewLog_DeleteOldConversationMessageLog_Import");
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("2021-04-06 02:49:57", true)]
        public void DeleteNotificationTypeCommand_IsValid_CheckCorrectly(string currentDate, bool expectedResult)
        {
            // Arrange
            var command = new DeleteOldConversationMessageLogCommand { CurrentDate = currentDate };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
