namespace Fanex.NCB.Criteria.Tests.ResultAmendmentLicensee
{
    using Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentLicensee;
    using Xunit;

    public class ImportResultAmendmentLicenseeCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new ImportResultAmendmentLicenseeCommand();
            var expected = "NCB_ResultAmendment_Licensee_Import";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new ImportResultAmendmentLicenseeCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
