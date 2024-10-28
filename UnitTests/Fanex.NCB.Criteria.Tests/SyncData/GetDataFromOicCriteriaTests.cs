namespace Fanex.NCB.Criteria.Tests.SyncData
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SyncData;
    using Xunit;

    public class GetDataFromOicCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataFromOicCriteria();
            var expected = "NAP_Chatbot_GetDataToSync";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataFromOicCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}