namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Xunit;

    public class GetAbnormalBetMessageDetailCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetAbnormalBetMessageDetailCriteria();
            var expected = "NAP_Chatbot_SpecialDelete_InternalCSD_MessageSel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAbnormalBetMessageDetailCriteria
            {
                MessageIdLst = "1"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnFalse()
        {
            // Arrange
            var criteria = new GetAbnormalBetMessageDetailCriteria
            {
                MessageIdLst = ""
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}