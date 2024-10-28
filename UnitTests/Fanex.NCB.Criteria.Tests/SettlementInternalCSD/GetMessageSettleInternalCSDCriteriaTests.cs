namespace Fanex.NCB.Criteria.Tests.SettlementInternalCSD
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SettlementInternalCSD;
    using Xunit;

    public class GetMessageSettleInternalCSDCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMessageSettleInternalCSDCriteria();
            var expected = "NAP_Chatbot_Settlement_InternalCSD_MessageSel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMessageSettleInternalCSDCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
