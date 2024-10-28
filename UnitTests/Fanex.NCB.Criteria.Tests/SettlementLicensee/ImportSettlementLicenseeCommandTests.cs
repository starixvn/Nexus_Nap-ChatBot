namespace Fanex.NCB.Criteria.Tests.SettlementLicensee
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SettlementLicensee;
    using Xunit;

    public class ImportSettlementLicenseeCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new ImportSettlementLicenseeCommand();
            var expected = "NCB_Settlement_Licensee_Import";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new ImportSettlementLicenseeCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
