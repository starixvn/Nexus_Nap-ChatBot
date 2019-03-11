using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Core.Controllers;
using Fanex.NCB.Bot.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Fanex.NCB.Bot.Core.UnitTests.Controllers
{
    public class LicBotControllerTests
    {
        private readonly IMock<IBotConversationRepository> _botConversationRepository;
        private readonly IMock<IBotConversationMemberRepository> _botConversationMemberRepository;
        private readonly BotConfiguration _botConfiguration;
        private readonly Mock<IBotAdapterFactory> _botAdapterFactory;
        public LicBotControllerTests()
        {
            _botConversationMemberRepository = new Mock<IBotConversationMemberRepository>();
            _botConversationRepository = new Mock<IBotConversationRepository>();
            _botConfiguration = new BotConfiguration
            {
                Name = "test",
                Version = "1.0",
                Description = "test bot",
                Services = new List<ConnectedService> { new ConnectedService("endpoint") }
            };
            _botAdapterFactory = new Mock<IBotAdapterFactory>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task PostAsync_ThrowInValidRequestException(string lb)
        {
            // Arrange
            var controller = new LicBotController(_botConfiguration,
                _botConversationRepository.Object,
                _botConversationMemberRepository.Object,
                _botAdapterFactory.Object
                );

            //Action

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await controller.PostAsync(lb).ConfigureAwait(false)).ConfigureAwait(false);

            //Assert

            Assert.Contains("Invalid request!The bot name is null or empty", exception.Message);

        }

        [Fact]
        public async Task PostAsync_CreateAdapter_ThrowInvalidOperationException()
        {
            // Arrange

            const string botName = "sparta";
            var expectedMessage = $"The .bot file does not contain an endpoint with name '{botName}'.";
            var controller = new LicBotController(_botConfiguration,
                _botConversationRepository.Object,
                _botConversationMemberRepository.Object,
                new BotAdapterFactory()
                )
            {
                ControllerContext = ArrangeControllerContext(new Activity { TopicName = "test topic" })
            };
            //Action

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await controller.PostAsync(botName).ConfigureAwait(false)).ConfigureAwait(false);

            //Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Contains(expectedMessage, exception.Message);

        }
        [Fact]
        public async Task PostAsync_ReturnHttpStatus_WithOKValue()
        {
            // Arrange
            var activity = new Activity { TopicName = "test topic" };

            ArrangeAdapterIntegration(null);

            var controller = new LicBotController(_botConfiguration,
                _botConversationRepository.Object,
                _botConversationMemberRepository.Object,
                _botAdapterFactory.Object
                )
            {
                ControllerContext = ArrangeControllerContext(activity)
            };
            //Action

            await controller.PostAsync("test").ConfigureAwait(false);

            //Assert
            Assert.True(controller.Response.StatusCode == (int)HttpStatusCode.OK);
        }
        [Theory]
        [InlineData(200)]
        [InlineData(500)]
        public async Task PostAsync_ReturnCorrectHttpStatus(int statusCode)
        {
            // Arrange
            var activity = new Activity { TopicName = "test topic" };
            var invokeResponse = new InvokeResponse
            {
                Status = statusCode,
                Body = activity
            };

            ArrangeAdapterIntegration(invokeResponse);
            var controller = new LicBotController(_botConfiguration,
                _botConversationRepository.Object,
                _botConversationMemberRepository.Object,
                _botAdapterFactory.Object
                )
            {
                ControllerContext = ArrangeControllerContext(activity)
            };
            //Action

            await controller.PostAsync("test").ConfigureAwait(false);

            //Assert
            Assert.True(controller.Response.StatusCode == statusCode);
            Assert.Equal("application/json", controller.Response.ContentType);
        }
        private void ArrangeAdapterIntegration(InvokeResponse invokeResponse)
        {
            var adapter = new Mock<IAdapterIntegration>();
            adapter.Setup(x => x.ProcessActivityAsync(It.IsAny<string>(), It.IsAny<Activity>(), It.IsAny<BotCallbackHandler>(),
                default(CancellationToken))).ReturnsAsync(invokeResponse);
            _botAdapterFactory.Setup(x => x.Create(It.IsAny<BotConfiguration>(), It.IsAny<string>())).Returns(adapter.Object);
        }

        private ControllerContext ArrangeControllerContext(Activity input)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(input));
            var stream = new MemoryStream(data);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            return new ControllerContext()
            {
                HttpContext = httpContext,
            };
        }

    }
}
