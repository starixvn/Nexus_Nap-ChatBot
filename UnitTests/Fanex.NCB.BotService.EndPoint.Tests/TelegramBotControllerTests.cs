namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
    using global::Telegram.Bot;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class TelegramBotControllerTests
    {
        private IOptions<List<TelegramBotConfigurations>> botConfigurations;
        private ITelegramBotService telegramBotService;

        [Fact]
        public async Task SetWebhook_WhenCalledSuccess_ReturnOkResultWithCorrectMessag()
        {
            var controller = GetController();

            var actual = await controller.SetWebhook();

            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Successfully!", okResult.Value);
        }

        [Fact]
        public async Task SetWebhook_WhenCalledThrowException_ReturnOkResultWithExceptionMesssage()
        {
            var controller = GetController();
            telegramBotService.SetWebhookAsync().Throws(new Exception("Error"));
            var actual = await controller.SetWebhook();

            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Error", okResult.Value);
        }

        [Fact]
        public async Task HealthCheck_WhenCalledSuccess_ReturnOkResultWithCorrectMessag()
        {
            var controller = GetController();

            var actual = await controller.HealthCheck();

            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Bots are healthy!", okResult.Value);
        }

        [Fact]
        public async Task HealthCheck_WhenCalledThrowException_ReturnOkResultWithExceptionMesssage()
        {
            var controller = GetController();
            telegramBotService.GetBotInfoAsync(Arg.Any<long>()).Throws(new Exception("Error"));
            var actual = await controller.HealthCheck();

            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Error", okResult.Value);
        }

        private TelegramBotController GetController()
        {
            botConfigurations = Substitute.For<IOptions<List<TelegramBotConfigurations>>>();
            telegramBotService = Substitute.For<ITelegramBotService>();
            botConfigurations.Value.Returns(new List<TelegramBotConfigurations>()
            {
                new() { BotId = 1, Token = "abc", BotClient = Substitute.For<ITelegramBotClient>()}
            });
            return new TelegramBotController(botConfigurations, telegramBotService);
        }
    }
}