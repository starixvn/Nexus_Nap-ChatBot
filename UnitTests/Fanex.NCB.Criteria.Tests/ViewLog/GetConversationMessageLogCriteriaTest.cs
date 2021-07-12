namespace Fanex.NCB.Criteria.Tests.ViewLog
{
    using AutoFixture;
    using Fanex.NCB.Criteria.ViewLog;
    using FluentAssertions;
    using Xunit;

    public class GetConversationMessageLogCriteriaTest
    {
        [Fact]
        public void GetConversationMessageLogCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetConversationMessageLogCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_ViewLog_ConversationMessageLog_Sel");
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        public void GetConversationMessageLogCriteria_IsValid_CheckCorrectly(int pageIndex, int pageSize, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var command = fixture.Create<GetConversationMessageLogCriteria>();
            command.PageIndex = pageIndex;
            command.PageSize = pageSize;

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
