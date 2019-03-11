using System;
using System.Threading.Tasks;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fanex.NCB.Bot.MySQLStorage.UnitTests
{
    public class BotConversationRepositoryTests : IDisposable
    {
        private readonly BotServiceDBContext botServiceDBContext;
        public BotConversationRepositoryTests()
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
            var item = CreateBotConversation();
            var repository = new BotConversationRepository(botServiceDBContext);

            // Act
            var result = await repository.InsertAsync(item).ConfigureAwait(false);

            // Assert

            Assert.True(result > 0);

        }

        private static BotConversation CreateBotConversation()
        {
            return new BotConversation
            {
                UpdatedBy = "sparta",
                ServiceUrl = "test",
                ChannelID = "test",
                ConversationID = "test test",
                LastUpdate = DateTime.Now,
                Name = "test group"
            };
        }

        [Fact]
        public async Task InsertAsync_WhenCall_ThrowException()
        {
            //Arrange
            var item = CreateBotConversation();
            item.ConversationID = null;
            var repository = new BotConversationRepository(botServiceDBContext);
            // Act 
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await repository.InsertAsync(item).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert

            Assert.Contains("property 'ConversationID' is null", ex.Message);

        }

        [Fact]
        public async Task SingleAsync_WhenCall_ReturnNull()
        {
            //Arrange
            var repository = new BotConversationRepository(botServiceDBContext);

            // Act
            var result = await repository.SingleAsync(x => x.ConversationID == "noitem").ConfigureAwait(false);

            // Assert

            Assert.True(result == null);
        }

        [Fact]
        public async Task SingleAsync_WhenCall_ReturnCorrectValue()
        {
            //Arrange
            var item = CreateBotConversation();
            var repository = new BotConversationRepository(botServiceDBContext);
            var groupId = await repository.InsertAsync(item);

            // Act
            var result = await repository.SingleAsync(x => x.GroupID == groupId).ConfigureAwait(false);

            // Assert

            Assert.True(result != null && result.ConversationID == item.ConversationID);
        }

        [Fact]
        public async Task UpdateAsync_WhenCall_ReturnSuccessful()
        {
            //Arrange
            var item = CreateBotConversation();
            var repository = new BotConversationRepository(botServiceDBContext);
            var groupId = await repository.InsertAsync(item);
            const string newBotName = "updated Bot Name";
            item.UpdatedBy = newBotName;
            item.GroupID = groupId;
            // Act
            var result = await repository.UpdateAsync(item).ConfigureAwait(false);
            var updatedItem = await repository.SingleAsync(x => x.GroupID == groupId).ConfigureAwait(false);
            // Assert

            Assert.True(result);
            Assert.Equal(newBotName, updatedItem.UpdatedBy);
        }

        [Fact]
        public async Task UpdateAsync_WhenCall_ReturnFailed()
        {
            //Arrange
            var item = CreateBotConversation();
            var item2 = CreateBotConversation();
            item2.ConversationID = "NotMatch";
            var repository = new BotConversationRepository(botServiceDBContext);
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
