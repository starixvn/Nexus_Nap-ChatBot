namespace Fanex.NCB.Criteria.Tests.ProblemAccount
{
    using Fanex.NCB.AutoJobs.Core.Criteria.ProblemAccount;
    using Xunit;

    public class GetMessageSettingCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMessageSettingCriteria();
            var expected = "NCB_MsgSetting_ProblemAccounts";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMessageSettingCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
