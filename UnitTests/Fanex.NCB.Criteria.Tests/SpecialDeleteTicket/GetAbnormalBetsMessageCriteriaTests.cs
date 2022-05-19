namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Xunit;

    public class GetAbnormalBetsMessageCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetAbnormalBetsMessageCriteria();
            var expected = "NAP_Chatbot_SpecialDeleteMsg_AbnormalBets_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAbnormalBetsMessageCriteria
            {
                MessageIDLst = "1",
                BatchSize = 2,
                MsgContentsTypeLst = "test",
                Environment = "UAT"
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
            var criteria = new GetAbnormalBetsMessageCriteria
            {
                MessageIDLst = "1",
                BatchSize = 0,
                MsgContentsTypeLst = "",
                Environment = "UAT"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
