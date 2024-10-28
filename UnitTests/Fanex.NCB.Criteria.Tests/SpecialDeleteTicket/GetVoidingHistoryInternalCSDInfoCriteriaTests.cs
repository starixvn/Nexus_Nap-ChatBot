namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket
{
    using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
    using Xunit;

    public class GetVoidingHistoryInternalCSDInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetVoidingHistoryInternalCSDInfoCriteria();
            var expected = "NAP_Chatbot_SpecialDelete_InternalCSD_Info";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetVoidingHistoryInternalCSDInfoCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
