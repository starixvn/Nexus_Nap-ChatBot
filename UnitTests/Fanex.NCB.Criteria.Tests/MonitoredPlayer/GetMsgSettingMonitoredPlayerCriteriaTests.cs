namespace Fanex.NCB.Criteria.Tests.MonitoredPlayer
{
    using Fanex.NCB.AutoJobs.Core.Criteria.MonitoredPlayer;
    using Xunit;

    public class GetMsgSettingMonitoredPlayerCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMsgSettingMonitoredPlayerCriteria();
            var expected = "NCB_MsgSetting_MonitoredPlayer";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMsgSettingMonitoredPlayerCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
