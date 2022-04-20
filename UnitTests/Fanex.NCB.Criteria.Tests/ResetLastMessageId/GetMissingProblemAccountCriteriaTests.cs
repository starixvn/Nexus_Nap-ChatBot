namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId
{
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Xunit;

    public class GetMissingProblemAccountCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMissingProblemAccountCriteria();
            var expected = "NAP_Chatbot_MissingProblemAccounts_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMissingProblemAccountCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
