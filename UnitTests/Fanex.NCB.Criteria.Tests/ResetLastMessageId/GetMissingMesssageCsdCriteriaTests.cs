namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId
{
    using System;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Xunit;

    public class GetMissingMesssageCsdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMissingMesssageCsdCriteria();
            var expected = "NAP_Chatbot_AbnormalBets_MissingMesssageInfo_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMissingMesssageCsdCriteria()
            {
                FromMessageId = 1,
                ToMessageId = 2,
                PostTime = new DateTime(2021, 10, 28)
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
