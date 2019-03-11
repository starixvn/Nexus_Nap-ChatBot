using System;
using System.Threading.Tasks;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fanex.NCB.Bot.MySQLStorage.UnitTests
{
    public class BotConversationMemberRepositoryTests : IDisposable
    {
        private readonly BotServiceDBContext botServiceDBContext;
        public BotConversationMemberRepositoryTests()
        {
            var dbOptions = new DbContextOptionsBuilder<BotServiceDBContext>()
                         .UseInMemoryDatabase(Guid.NewGuid().ToString())
                         .EnableSensitiveDataLogging()
                         .Options;
            botServiceDBContext = new BotServiceDBContext(dbOptions);
            botServiceDBContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task InsertAsync_WhenCall_Successful()
        {
            //Arrange
            var item = CreateBotConversationMember();
            var repository = new BotConversationMemberRepository(botServiceDBContext);

            // Act
            var result = await repository.InsertAsync(item).ConfigureAwait(false);

            // Assert

            Assert.True(result > 0);

        }

        private static BotConversationMember CreateBotConversationMember()
        {
            return new BotConversationMember
            {
                UpdatedBy = "sparta",
                ConversationID = "test test",
                LastUpdate = DateTime.Now,
                Name = "test group",
                IsBot = true,
                IsActive = true,
                MemberId = "random ID"
            };
        }

        [Theory]
        [InlineData("conversationid", null)]
        [InlineData(null, "memberid")]
        [InlineData(null, null)]
        public async Task InsertAsync_WhenCall_ThrowException(string conversationID, string memberId)
        {
            //Arrange
            var item = CreateBotConversationMember();
            item.ConversationID = conversationID;
            item.MemberId = memberId;
            var exptecdMessage = conversationID == null ? $"property '{nameof(conversationID)}' is null" : $"property '{nameof(memberId)}' is null";
            var repository = new BotConversationMemberRepository(botServiceDBContext);
            // Act 
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await repository.InsertAsync(item).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert

            Assert.Contains(exptecdMessage, ex.Message, StringComparison.CurrentCultureIgnoreCase);

        }

        [Fact]
        public async Task SingleAsync_WhenCall_ReturnNull()
        {
            //Arrange
            var repository = new BotConversationMemberRepository(botServiceDBContext);

            // Act
            var result = await repository.SingleAsync(x => x.ConversationID == "noitem").ConfigureAwait(false);

            // Assert

            Assert.True(result == null);
        }

        [Fact]
        public async Task SingleAsync_WhenCall_ReturnCorrectValue()
        {
            //Arrange
            var item = CreateBotConversationMember();
            var repository = new BotConversationMemberRepository(botServiceDBContext);
            await repository.InsertAsync(item).ConfigureAwait(false);

            // Act
            var result = await repository.SingleAsync(x => x.ConversationID == item.ConversationID && x.MemberId == item.MemberId)
                .ConfigureAwait(false);

            // Assert

            Assert.True(result != null);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateAsync_WhenCall_ReturnSuccessful(bool isActive)
        {
            //Arrange
            var item = CreateBotConversationMember();
            var repository = new BotConversationMemberRepository(botServiceDBContext);
            await repository.InsertAsync(item).ConfigureAwait(false);

            item.LastUpdate = DateTime.Now;
            item.IsActive = isActive;
            // Act
            var result = await repository.UpdateAsync(item).ConfigureAwait(false);
            var updatedItem = await repository.SingleAsync(x => x.ConversationID == item.ConversationID
            && x.MemberId == item.MemberId).ConfigureAwait(false);
            // Assert

            Assert.True(result);
            Assert.Equal(isActive, updatedItem.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_WhenCall_ReturnFailed()
        {
            //Arrange
            var item = CreateBotConversationMember();
            var item2 = CreateBotConversationMember();
            item2.ConversationID = "NotMatch";
            var repository = new BotConversationMemberRepository(botServiceDBContext);
            await repository.InsertAsync(item).ConfigureAwait(false);

            // Act
            var result = await repository.UpdateAsync(item2).ConfigureAwait(false);
            // Assert

            Assert.True(!result);

        }

        public void Dispose()
        {
            botServiceDBContext?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
