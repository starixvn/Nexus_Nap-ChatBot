namespace Fanex.NCB.Criteria.Tests.ResultAmendmentInternalCSD
{
    using Fanex.NCB.Criteria.ResultAmendmentInternalCSD;
    using Xunit;

    public class GetMessageResultAmendmentCsdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMessageResultAmendmentCsdCriteria();
            var expected = "NAP_Chatbot_ResultAmendment_InternalCSD_MessageSel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMessageResultAmendmentCsdCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
