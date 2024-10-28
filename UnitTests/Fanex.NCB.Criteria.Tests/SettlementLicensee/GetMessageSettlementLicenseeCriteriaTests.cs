namespace Fanex.NCB.Criteria.Tests.SettlementLicensee
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SettlementLicensee;
    using Xunit;

    public class GetMessageSettlementLicenseeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMessageSettlementLicenseeCriteria();
            var expected = "NAP_Chatbot_Settlement_Licensee_MessageSel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMessageSettlementLicenseeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
