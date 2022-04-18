namespace Fanex.NCB.Criteria.Tests.ProblemAccount
{
    using Fanex.NCB.Criteria.ProblemAccount;
    using Xunit;

    public class ImportFromProblemAccountCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new ImportFromProblemAccountCriteria();
            var expected = "NCB_ProblemAccount_Import";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new ImportFromProblemAccountCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
