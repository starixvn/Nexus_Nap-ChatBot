using System;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Skype.Controllers;
using Microsoft.Bot.Configuration;
using Moq;
using Xunit;

namespace Fanex.NCB.Bot.Skype.UnitTests.Controllers
{
    public class LicBotControllerTests
    {
        private readonly IMock<IBotConversationRepository> _botConversationRepository;
        private readonly IMock<IBotConversationMemberRepository> _botConversationMemberRepository;
        private readonly BotConfiguration _botConfiguration;
        public LicBotControllerTests()
        {
            _botConversationMemberRepository = new Mock<IBotConversationMemberRepository>();
            _botConversationRepository = new Mock<IBotConversationRepository>();
            _botConfiguration = new BotConfiguration
            {
                Name = "test",
                Version = "1.0",
                Description = "test bot"
            };
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task PostAsync_ThrowInValidRequestException(string lb)
        {
            // Arrange
            var controller = new LicBotController(_botConfiguration, _botConversationRepository.Object, _botConversationMemberRepository.Object);

            //Action

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await controller.PostAsync(lb).ConfigureAwait(false)).ConfigureAwait(false);

            //Assert

            Assert.Contains("Invalid request!The bot name is null or empty", exception.Message);


        }
    }
}
