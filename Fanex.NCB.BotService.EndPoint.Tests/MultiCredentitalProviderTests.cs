namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Configuration;
    using Xunit;

    [Obsolete]
    public class MultiCredentitalProviderTests
    {
        [Theory]
        [InlineData("BotId", false)]
        [InlineData("Bot_1", true)]
        public async Task IsValidAppIdAsync_WhenCalled_ReturnExpectedResult(string appId, bool expected)
        {
            var provider = SetUp();

            //Act
            var actual = await provider.IsValidAppIdAsync(appId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAppPasswordAsync_WhenExistsAppId_ReturnPassword()
        {
            var provider = SetUp();

            //Act
            var actual = await provider.GetAppPasswordAsync("Bot_1");

            Assert.Equal("Pass1", actual);
        }

        [Fact]
        public async Task GetAppPasswordAsync_WhenNotExistsAppId_ReturnNull()
        {
            var provider = SetUp();

            //Act
            var actual = await provider.GetAppPasswordAsync("Bot");

            Assert.Null(actual);
        }

        [Fact]
        public async Task IsAuthenticationDisabledAsync_WhenCalled_ReturnExpected()
        {
            var provider = SetUp();

            //Act
            var actual = await provider.IsAuthenticationDisabledAsync();

            Assert.False(actual);
        }

        [Obsolete]
        private MultiCredentialProvider SetUp()
        {
            var listBotConfig = new List<ConnectedService>
            {
                new EndpointService {AppId = "Bot_1", AppPassword = "Pass1"}
            };

            var botConfig = new BotConfiguration
            {
                Services = listBotConfig
            };
            return new MultiCredentialProvider(botConfig);
        }
    }
}