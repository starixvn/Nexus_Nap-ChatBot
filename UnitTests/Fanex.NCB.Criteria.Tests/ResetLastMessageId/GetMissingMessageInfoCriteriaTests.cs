namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId
{
    using System;
    using System.Collections.Generic;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Xunit;

    public class GetMissingMessageInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMissingMessageInfoCriteria();
            var expected = "NAP_Chatbot_MissingMesssageInfo_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMissingMessageInfoCriteria()
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
            var messages = new List<MessageInfo>
            {
                new MessageInfo { JobIdentify = 1, FromMessageId = 123, ToMessageId = 456, PostTime = new DateTime(2021, 10, 28) }
            };
            var messageListXml = "<root><r JobIdentify='1' FromMessageId='123' ToMessageId='456' PostTime='10/28/2021 12:00:00 AM'/></root>";

            // Act
            var criteria = new GetMissingMessageInfoCriteria
            {
                Messages = messages
            };

            // Assert
            Assert.Equal(messageListXml, criteria.MessageListXml);
        }

        [Fact]
        public void MsgContentsTypeListXml_MsgContentTypesHaveData_ReturnAsExpected()
        {
            // Arrange
            var msgContentTypes = new List<MessageTypeInfo>
            {
                new MessageTypeInfo { JobIdentify = 1, MsgContentsType = 123 }
            };
            var msgContentsTypeListXml = "<root><r JobIdentify='1' MsgContentsType='123'/></root>";

            // Act
            var criteria = new GetMissingMessageInfoCriteria
            {
                MsgContentTypes = msgContentTypes
            };

            // Assert
            Assert.Equal(msgContentsTypeListXml, criteria.MsgContentsTypeListXml);
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
            var criteria = new GetMissingMessageInfoCriteria
            {
                Requests = requests
            };

            // Assert
            Assert.Equal(requestListXml, criteria.RequestListXml);
        }
    }
}
