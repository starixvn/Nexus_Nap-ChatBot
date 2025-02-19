namespace Fanex.NCB.Criteria.Tests.ViewLog;

using AutoFixture;
using Fanex.NCB.Criteria.ViewLog;
using FluentAssertions;
using Xunit;

public class GetNotificationRecipientListCriteriaTest
{
    [Fact]
    public void GetNotificationRecipientListCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetNotificationRecipientListCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_ViewLog_NotificationRecipientList_Sel");
    }

    [Theory]
    [InlineData(0, 1, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 1, true)]
    public void GetNotificationRecipientListCriteria_IsValid_CheckCorrectly(int pageIndex, int pageSize,
        bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var command = fixture.Create<GetNotificationRecipientListCriteria>();
        command.PageIndex = pageIndex;
        command.PageSize = pageSize;

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}