namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId;

using Fanex.NCB.Criteria.ResetLastMessageId;
using Xunit;

public class GetMissingMonitoredPlayerCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetMissingMonitoredPlayerCriteria();
        var expected = "NAP_Chatbot_MissingMonitoredPlayer_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetMissingMonitoredPlayerCriteria();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}