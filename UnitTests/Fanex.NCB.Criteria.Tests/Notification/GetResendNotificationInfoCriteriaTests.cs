namespace Fanex.NCB.Criteria.Tests.Notification;

using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using Xunit;

public class GetResendNotificationInfoCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetResendNotificationInfoCriteria();
        var expected = "NCB_ResendNotification_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, true, true)]
    [InlineData(0, true, false)]
    public void IsValid_WhenCalled_ReturnExpected(int notificationId, bool isSendAll, bool expected)
    {
        // Arrange
        var criteria = new GetResendNotificationInfoCriteria
        {
            InNotificationId = notificationId,
            InIsSendAll = isSendAll
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        actual.Should().Be(expected);
    }
}