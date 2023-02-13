namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Xunit;

    public class GetListMessageAbnormalbetsFromYesterdayToCurrentCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetVoidingHIstoryCollectionIdCriteria();
            var expected = "NAP_Chatbot_SpecialDelete_AbnormalBets_GetMessage";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetVoidingHIstoryCollectionIdCriteria { BatchSize = 10 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}