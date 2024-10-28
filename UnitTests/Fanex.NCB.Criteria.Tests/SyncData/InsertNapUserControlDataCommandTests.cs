namespace Fanex.NCB.Criteria.Tests.SyncData
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SyncData;
    using Xunit;

    public class InsertNapUserControlDataCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new InsertNapUserControlDataCommand();
            var expected = "NAP_UserControl_SyncData";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new InsertNapUserControlDataCommand
            {
                Functions = "Functions",
                Users = "Users"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}