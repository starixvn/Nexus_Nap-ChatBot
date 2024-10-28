namespace Fanex.NCB.Criteria.Tests.SyncData
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SyncData;
    using Xunit;

    public class InsertOicDataCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new InsertOicDataCommand();
            var expected = "NCB_OIC_SyncData";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new InsertOicDataCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}