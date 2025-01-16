namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Shared.Extensions;
    using Xunit;
    using Xunit.Sdk;

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
            var criteria = new GetMissingMessageInfoCriteria();

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
                new CurrentMaxMessageInfo { JobIdentify = 1, ReferenceValue = "123" }
            };
            var messageListXml = XmlHelper<CurrentMaxMessageInfo>.Serialize(messages);

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
            var msgContentsTypeListXml = XmlHelper<MessageTypeInfo>.Serialize(msgContentTypes); ;

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
            var requestListXml = XmlHelper<MessageRequestId>.Serialize(requests); ;

            // Act
            var criteria = new GetMissingMessageInfoCriteria
            {
                Requests = requests
            };

            // Assert
            Assert.Equal(requestListXml, criteria.VoidingCollectionRequestListXml);
        }

        [Fact]
        public void VoidingHistoryRequestListXml_RequestsHaveData_ReturnAsExpected()
        {
            // Arrange
            var messageIds = new List<int>(new int[] { 1 });

            var voidingHistoryRequestListXml = XmlHelper<int>.Serialize(messageIds); ;

            // Act
            var criteria = new GetMissingMessageInfoCriteria
            {
                MessageIds = messageIds
            };

            // Assert
            Assert.Equal(voidingHistoryRequestListXml, criteria.VoidingHistoryRequestListXml);
        }
    }
}