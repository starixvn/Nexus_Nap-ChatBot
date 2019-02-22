using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
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
                BotName = "sparta",
                ServiceUrl = "test",
                ChannelID = "test",
                ConversationID = "test test",
                IsBotActive = true,
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
            var ex = Assert.ThrowsAnyAsync<DbUpdateException>(async () => await repository.InsertAsync(item).ConfigureAwait(false));

            // Assert

            Assert.Contains("property 'ConversationID' is null", ex.Exception.Message);

        }

        public void Dispose()
        {
            botServiceDBContext?.Dispose();
        }
    }
}
