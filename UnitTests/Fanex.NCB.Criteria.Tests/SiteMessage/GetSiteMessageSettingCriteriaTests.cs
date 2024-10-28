namespace Fanex.NCB.Criteria.Tests.SiteMessage
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;
    using Xunit;

    public class GetSiteMessageSettingCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetSiteMessageSettingCriteria();
            var expected = "NCB_MsgSetting_SiteMessage_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetSiteMessageSettingCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
