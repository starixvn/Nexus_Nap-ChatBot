namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Model.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class ChatGroupMappingServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetChatGroupMappingListAsync_WhenCalled_ReturnExpectedResult()
        {
            var model = new GetChapGroupMapping { BotIDs = "1,2,3" };
            var data = FakeTupleValue();
            var criteria = new GetChapGroupMappingCriteria { TotalItems = 10 };
            var service = GetService();

            baseService.FetchMultipleAsync<BotConversation, BotConversationMember, ConversationRecipient>(Arg.Do<GetChapGroupMappingCriteria>(x => criteria = x)).Returns(data);

            //Act
            var actual = await service.GetChatGroupMappingListAsync(model).ConfigureAwait(false);

            //Assert
            Assert.Equal(criteria.TotalItems, actual.Item2);
            Assert.Equal("Test", actual.Item1.FirstOrDefault().ConversationID);
            Assert.Equal(1, actual.Item1.FirstOrDefault().GroupID);
            Assert.Equal(2, actual.Item1.FirstOrDefault().RecipientTypeID);
            Assert.Equal("1on1Notify", actual.Item1.FirstOrDefault().BotName);
        }

        [Fact]
        public async Task EditAsync_IsSuccessful_ReturnExpectedCommandResult()
        {
            var model = new EditChatGroupMapping { GroupID = 1 };
            var command = new EditChatGroupMappingCommand();
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<EditChatGroupMappingCommand>(x => command = x)).Returns(commandResult);

            //Act
            var actual = await service.EditAsync(model).ConfigureAwait(false);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        private ValueTuple<IEnumerable<BotConversation>,
            IEnumerable<BotConversationMember>,
            IEnumerable<ConversationRecipient>>
            FakeTupleValue()
        {
            var botConversations = new List<BotConversation>
            {
                new BotConversation {GroupID = 1, ConversationID = "Test", RecipientTypeId = 2}
            };
            var members = new List<BotConversationMember>
            {
                new BotConversationMember { ConversationID = "Test", BotName = "1on1Notify"}
            };
            var recipients = new List<ConversationRecipient>
            {
                new ConversationRecipient { RecipientID = 2, GroupID = 1}
            };

            return new ValueTuple<IEnumerable<BotConversation>,
            IEnumerable<BotConversationMember>,
            IEnumerable<ConversationRecipient>>(botConversations, members, recipients);
        }

        private ChatGroupMappingService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            return new ChatGroupMappingService(baseService);
        }
    }
}