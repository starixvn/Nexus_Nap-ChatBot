namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using FluentAssertions;
    using Xunit;

    public class GetAbnormalAccountUsernameLstCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetAbnormalAccountUsernameLstCriteria();
            var expected = "NAP_Chatbot_SpecialDelete_AbnormalBets_GetUsernameList";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("123", true)]

        public void IsValid_WhenCalled_ReturnExpected(string messageIDLst, bool expected)
        {
            // Arrange
            var criteria = new GetAbnormalAccountUsernameLstCriteria
            {
                MessageIDLst = messageIDLst
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
