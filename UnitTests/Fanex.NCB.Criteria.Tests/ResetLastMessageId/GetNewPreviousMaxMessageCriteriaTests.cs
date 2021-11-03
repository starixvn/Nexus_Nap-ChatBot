namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Xunit;

    public class GetNewPreviousMaxMessageCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetNewPreviousMaxMessageCriteria();
            var expected = "NAP_Chatbot_MaxMessageIdAndPostTime_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetNewPreviousMaxMessageCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void MessageListXml_MessagesHaveData_ReturnAsExpected()
        {
            // Arrange
            var messages = new List<int>
            {
                123,456
            };
            var messageListXml = "<root><r MessageId='123'/><r MessageId='456'/></root>";

            // Act
            var criteria = new GetNewPreviousMaxMessageCriteria
            {
                Messages = messages
            };

            // Assert
            Assert.Equal(messageListXml, criteria.MessageListXml);
        }
    }
}
