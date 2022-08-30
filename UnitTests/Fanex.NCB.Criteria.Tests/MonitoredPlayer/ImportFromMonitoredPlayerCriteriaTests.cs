namespace Fanex.NCB.Criteria.Tests.MonitoredPlayer
{
    using Fanex.NCB.Criteria.MonitoredPlayer;
    using Xunit;

    public class ImportFromMonitoredPlayerCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new ImportFromMonitoredPlayerCriteria();
            var expected = "NCB_MonitoredPlayer_Import";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new ImportFromMonitoredPlayerCriteria()
            {
                Message = "[{}]",
                ServerName = "TestDb2053",
                LastDataRow = "1",
                PLastLogId = 1,
                ErrCode = 0,
                ErrMsg = ""
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
