namespace Fanex.NCB.Criteria.Tests.SettingConfiguration;

using Fanex.NCB.Criteria.SettingConfiguration;
using FluentAssertions;
using Xunit;

public class GetNotificationTypeDataCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetNotificationTypeDataCriteria();
        var expected = "NCB_SettingConfiguration_NotificationTypeData_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void IsValid_WhenCalled_ReturnExpected(int notificationTypeId, bool expected)
    {
        // Arrange
        var criteria = new GetNotificationTypeDataCriteria
        {
            NotificationTypeId = notificationTypeId
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        actual.Should().Be(expected);
    }
}