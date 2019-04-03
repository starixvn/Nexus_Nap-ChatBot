using AutoFixture;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.ChatGroupMapping
{
    public class ChatGroupMappingService_GetChatGroupMappingListAsyncTest
    {
        [Fact]
        public async Task GetChatGroupMappingListAsync_WhenCalled_AssignCountCorrectly()
        {
            // Arrange
            var chatGroupMappingStorage = new Mock<IChatGroupMappingStorage>();
            var chatGroupMappingResultService = new Mock<IChatGroupMappingResultService>();
            var service = new ChatGroupMappingService(chatGroupMappingStorage.Object, chatGroupMappingResultService.Object);

            var count = 100;
            var criteria = new GetChapGroupMappingCriteria();
            var fakeData = Tuple.Create<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>(
                count, new List<BotConversation>(), new List<BotConversationMember>(), new List<ConversationRecipient>());
            chatGroupMappingStorage.Setup(s => s.GetChatGroupMappingListAsync(criteria)).Returns(Task.FromResult(fakeData));

            // Act
            var result = await service.GetChatGroupMappingListAsync(criteria);

            // Assert
            result.Count.Should().Be(count);
        }

        [Fact]
        public async Task GetChatGroupMappingListAsync_WhenCalled_GetDataFromBuildChatGroupMappingResult()
        {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingStorage = new Mock<IChatGroupMappingStorage>();
            var chatGroupMappingResultService = new Mock<IChatGroupMappingResultService>();
            var service = new ChatGroupMappingService(chatGroupMappingStorage.Object, chatGroupMappingResultService.Object);

            var count = 2;
            var conversationList = fixture.CreateMany<BotConversation>(count).ToList();
            var chatGroupMappingResult = fixture.Create<ChatGroupMappingResult>();
            var criteria = new GetChapGroupMappingCriteria();
            var fakeData = Tuple.Create<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>(
                count, conversationList, new List<BotConversationMember>(), new List<ConversationRecipient>());
            chatGroupMappingStorage.Setup(s => s.GetChatGroupMappingListAsync(criteria)).Returns(Task.FromResult(fakeData));
            chatGroupMappingResultService.Setup(s => s.BuildChatGroupMappingResult(It.IsAny<BotConversation>(), It.IsAny<BotConversationMember>(), It.IsAny<List<ConversationRecipient>>()))
                .Returns(chatGroupMappingResult);

            var expectedData = new List<ChatGroupMappingResult>
            {
                chatGroupMappingResult,
                chatGroupMappingResult
            };

            // Act
            var result = await service.GetChatGroupMappingListAsync(criteria);

            // Assert
            result.Data.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetChatGroupMappingListAsync_WhenCalled_GetConversationMemberCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingStorage = new Mock<IChatGroupMappingStorage>();
            var chatGroupMappingResultService = new Mock<IChatGroupMappingResultService>();
            var service = new ChatGroupMappingService(chatGroupMappingStorage.Object, chatGroupMappingResultService.Object);

            var criteria = new GetChapGroupMappingCriteria();
            var count = 1;
            var conversationID = fixture.Create<string>();
            var groupID = fixture.Create<int>();
            var conversationList = fixture.CreateMany<BotConversation>(count).ToList();
            var conversationMemberList = fixture.CreateMany<BotConversationMember>(2).ToList();
            var conversationRecipientList = fixture.CreateMany<ConversationRecipient>(3).ToList();
            conversationList[0].ConversationID = conversationID;
            conversationList[0].GroupID = groupID;
            conversationMemberList[1].ConversationID = conversationID;
            conversationRecipientList[1].GroupID = groupID;
            conversationRecipientList[2].GroupID = groupID;

            var fakeData = Tuple.Create<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>(
                count, conversationList, conversationMemberList, conversationRecipientList);
            chatGroupMappingStorage.Setup(s => s.GetChatGroupMappingListAsync(criteria)).Returns(Task.FromResult(fakeData));

            BotConversation botConversation = null;
            BotConversationMember botConversationMember = null;
            List<ConversationRecipient> conversationRecipients = null;
            chatGroupMappingResultService.Setup(s => s.BuildChatGroupMappingResult(It.IsAny<BotConversation>(), It.IsAny<BotConversationMember>(), It.IsAny<List<ConversationRecipient>>()))
                .Callback<BotConversation, BotConversationMember, List<ConversationRecipient>>((p1, p2, p3) => { botConversation = p1; botConversationMember = p2; conversationRecipients = p3; });
            var expectedConversationRecipients = new List<ConversationRecipient> { conversationRecipientList[1], conversationRecipientList[2] };

            // Act
            await service.GetChatGroupMappingListAsync(criteria);

            // Assert
            botConversationMember.Should().BeEquivalentTo(conversationMemberList[1]);
            botConversation.Should().BeEquivalentTo(conversationList[0]);
            conversationRecipients.Should().BeEquivalentTo(expectedConversationRecipients);
        }
    }
}
