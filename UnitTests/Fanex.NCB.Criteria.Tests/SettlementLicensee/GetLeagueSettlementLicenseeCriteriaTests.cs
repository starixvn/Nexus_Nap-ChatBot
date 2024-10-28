namespace Fanex.NCB.Criteria.Tests.SettlementLicensee
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SettlementLicensee;
    using Xunit;

    public class GetLeagueSettlementLicenseeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetLeagueSettlementLicenseeCriteria();
            var expected = "NAP_Chatbot_Settlement_Licensee_LeagueSel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetLeagueSettlementLicenseeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
