namespace Fanex.NCB.Criteria.Tests.SettlementInternalCSD
{
    using Fanex.NCB.Criteria.SettlementInternalCSD;
    using Xunit;

    public class GetEventSettleInternalCSDCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetEventSettleInternalCSDCriteria();
            var expected = "NAP_Chatbot_Settlement_InternalCSD_EventInfoSel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEventSettleInternalCSDCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
