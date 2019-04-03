using AutoFixture;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.ChatGroupMappingResultTest
{
    public class ChatGroupMappingResultService_BuildChatGroupMappingResultTest
    {
        [Fact]
        public void BuildChatGroupMappingResult_WhenCalled_BuildChatGroupMappingResultCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var mappingStatusService = new Mock<IMappingStatusService>();
            var service = new ChatGroupMappingResultService(mappingStatusService.Object);

            var mappingStatusID = fixture.Create<int>();
            var mappingStatusName = fixture.Create<string>();
            mappingStatusService.Setup(s => s.CalculateMappingStatus(It.IsAny<bool>(), It.IsAny<BotConversationMember>())).Returns(mappingStatusID);
            mappingStatusService.Setup(s => s.GetMappingStatusName(It.IsAny<int>())).Returns(mappingStatusName);

            var botConversation = fixture.Create<BotConversation>();
            var botConversationMember = fixture.Create<BotConversationMember>();
            var conversationRecipients = fixture.CreateMany<ConversationRecipient>(2).ToList();

            var expectedResult = new Model.DTO.ChatGroupMapping.ChatGroupMappingResult
            {
                BotName = botConversationMember?.BotName,
                GroupID = botConversation.GroupID,
                ConversationID = botConversation.ConversationID,
                GroupName = botConversation.GroupName,
                ChatGroupTypeID = botConversation.ChatGroupTypeID,
                ChatGroupTypeName = botConversation.ChatGroupTypeName,
                RecipientTypeID = botConversation.RecipientTypeId,
                RecipientTypeName = botConversation.RecipientTypeName,
                RecipientIDs = String.Join(",", conversationRecipients.Select(x => x.RecipientID).ToList()),
                RecipientNames = String.Join(", ", conversationRecipients.Select(x => x.RecipientName).ToList()),
                RecipientStatusNames = String.Join(", ", conversationRecipients.Where(x => x.RecipientStatusName != string.Empty).Select(x => x.RecipientStatusName).ToList()),
                MappingStatusID = mappingStatusID,
                MappingStatusName = mappingStatusName,
                Remark = botConversation.Remark
            };

            // Act
            var result = service.BuildChatGroupMappingResult(botConversation, botConversationMember, conversationRecipients);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
