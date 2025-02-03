namespace Fanex.NCB.Criteria.Tests.MonitoredPlayer;

using Fanex.NCB.AutoJobs.Core.Criteria.MonitoredPlayer;
using Xunit;

public class GetMonitoredPlayerInfoCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetMonitoredPlayerInfoCriteria();
        var expected = "NAP_Chatbot_MonitoredPlayer_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetMonitoredPlayerInfoCriteria()
        {
            LastLogId = 1,
            BatchSize = 1000,
            LastDataRow = "<root/>",
            ServerName = "TestDb2053"
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}