namespace Fanex.NCB.Criteria.Tests.SendNotification;

using Fanex.NCB.Criteria.SendNotification;
using Xunit;

public class GetNotificationToSendCriteriaTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetNotificationsToSendCriteria();
        var expected = "NCB_SendNotification_GetPlanRetryNotifUpdStatus";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_ParamsArevalid_ReturnTrue()
    {
        // Arrange
        var criteria = new GetNotificationsToSendCriteria();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}