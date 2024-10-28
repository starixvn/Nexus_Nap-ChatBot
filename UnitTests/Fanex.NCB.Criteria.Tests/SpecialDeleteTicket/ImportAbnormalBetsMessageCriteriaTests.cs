namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
    using Xunit;

    public class ImportAbnormalBetsMessageCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new ImportAbnormalBetsMessageCriteria();
            var expected = "NCB_SpecialDelete_InternalCSD_Import";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new ImportAbnormalBetsMessageCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
