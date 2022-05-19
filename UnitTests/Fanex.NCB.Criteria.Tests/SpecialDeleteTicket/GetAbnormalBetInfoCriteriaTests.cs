namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using FluentAssertions;
    using Xunit;

    public class GetAbnormalBetInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetAbnormalBetInfoCriteria();
            var expected = "NAP_Chatbot_SpecialDelete_AbnormalBets_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("123", true)]

        public void IsValid_WhenCalled_ReturnExpected(string messageIdLst, bool expected)
        {
            // Arrange
            var criteria = new GetAbnormalBetInfoCriteria
            {
                MessageIdLst = messageIdLst
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
