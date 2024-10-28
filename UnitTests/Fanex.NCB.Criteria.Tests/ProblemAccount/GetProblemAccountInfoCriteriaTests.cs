namespace Fanex.NCB.Criteria.Tests.ProblemAccount
{
    using Fanex.NCB.AutoJobs.Core.Criteria.ProblemAccount;
    using Xunit;

    public class GetProblemAccountInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetProblemAccountInfoCriteria();
            var expected = "NAP_Chatbot_ProblemAccounts_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetProblemAccountInfoCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
