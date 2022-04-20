namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Xunit;

    public class CreateMissingMessageCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new CreateMissingMessageCommand();
            var expected = "NAP_Chatbot_MissingMesssage_Ins";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateMissingMessageCommand()
            {
                Environment = "UAT"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void MessageListXml_MessagesHaveData_ReturnAsExpected()
        {
            // Arrange
            var messages = new List<CurrentMaxMessageInfo>
            {
                new CurrentMaxMessageInfo { JobIdentify = 1, ReferenceId = 123 }
            };
            var messageListXml = "<root><r JobIdentify='1' MessageId='123'/></root>";

            // Act
            var criteria = new CreateMissingMessageCommand
            {
                Messages = messages
            };

            // Assert
            Assert.Equal(messageListXml, criteria.MessageListXml);
        }

        [Fact]
        public void RequestListXml_RequestsHaveData_ReturnAsExpected()
        {
            // Arrange
            var requests = new List<MessageRequestId>
            {
                new MessageRequestId { MessageId = 1, RequestId = 123 }
            };
            var requestListXml = "<root><r MessageId='1' RequestId='123'/></root>";

            // Act
            var criteria = new CreateMissingMessageCommand
            {
                Requests = requests
            };

            // Assert
            Assert.Equal(requestListXml, criteria.RequestListXml);
        }
    }
}
