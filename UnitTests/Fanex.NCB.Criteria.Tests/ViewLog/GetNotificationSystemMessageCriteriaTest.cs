namespace Fanex.NCB.Criteria.Tests.ViewLog
{
    using AutoFixture;
    using Fanex.NCB.Criteria.ViewLog;
    using FluentAssertions;
    using Xunit;

    public class GetNotificationSystemMessageCriteriaTest
    {
        [Fact]
        public void GetNotificationSystemMessageCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetNotificationSystemMessageCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_ViewLog_NotificationSystemMessage_Sel");
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        public void GetNotificationSystemMessageCriteria_IsValid_CheckCorrectly(int pageIndex, int pageSize, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var command = fixture.Create<GetNotificationSystemMessageCriteria>();
            command.PageIndex = pageIndex;
            command.PageSize = pageSize;

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
