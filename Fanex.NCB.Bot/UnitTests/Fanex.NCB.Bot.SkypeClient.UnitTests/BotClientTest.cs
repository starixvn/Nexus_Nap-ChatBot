using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Moq;
using Newtonsoft.Json;

using Xunit;

namespace Fanex.NCB.Bot.SkypeClient.UnitTests
{
    public class BotClientTest
    {
        private readonly IMessageActivity _messageActivity;
        private readonly Mock<IConnectorClient> connectorClient;
        public BotClientTest()
        {
            _messageActivity = CreateTestActivityMessage("test message", "test id");
            var conversation = new Mock<IConversations>();
            conversation.Setup(x => x.SendToConversationWithHttpMessagesAsync(It.IsAny<string>()
                , It.IsAny<Activity>()
                , It.IsAny<Dictionary<string, List<string>>>()
                , It.IsAny<CancellationToken>()
                )).Returns(Task.FromResult(new Microsoft.Rest.HttpOperationResponse<ResourceResponse>()));

            connectorClient = new Mock<IConnectorClient>();
            connectorClient.Setup(x => x.Conversations).Returns(conversation.Object);
        }

        [Fact]
        public async Task SendAsync_MessageActivity_ReturnStatus_OK()
        {
            //Arrange
            var botClient = new Mock<BotClient>("any", "any") { CallBase = true };
            botClient.Setup(x => x.GetToken()).Returns(Task.FromResult(new Token { AccessToken = "any" }));
            botClient.Setup(x => x.SetupClientConnection()).Returns(Task.FromResult(connectorClient.Object));
            //Action

            var result = await botClient.Object.SendAsync(_messageActivity).ConfigureAwait(false);

            // Assert

            Assert.True(result == HttpStatusCode.OK);
        }

        [Fact]
        public async Task SendAsync_MessageActivity_ReturnStatus_Unauthorized()
        {
            //Arrange - Fake Server
            var botClient = new Mock<BotClient>("any", "any") { CallBase = true };
            botClient.Setup(x => x.GetToken()).Returns(Task.FromResult(new Token()));

            // Act
            var result = await botClient.Object.SendAsync(_messageActivity).ConfigureAwait(false);
            // Assert

            Assert.True(result == HttpStatusCode.Unauthorized);
        }

        private static IMessageActivity CreateTestActivityMessage(string message, string conversationId)
        {
            var item = Activity.CreateMessageActivity();
            item.Conversation = new ConversationAccount(id: conversationId);
            item.Text = message;
            return item;
        }

        [Theory]
        [InlineData("any", "")]
        [InlineData("", "123")]
        public async Task SendAsync_RawMessage_ReturnStatus_NoContent(string message, string conversationId)
        {
            //Arrange
            var client = new BotClient("any", "any");
            // Act
            var result = await client.SendAsync(message, conversationId);
            // Assert

            Assert.True(result == HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task SendAsync_RawMessage_CallingSendAsyncWithMessageActivity()
        {
            //Arrange
            var call = 0;
            var botClient = new Mock<BotClient>("any", "any") { CallBase = true };
            botClient.Setup(x => x.SendAsync(It.IsAny<IMessageActivity>())).Returns(Task.FromResult(HttpStatusCode.OK))
                .Callback(() => call++);
            // Act
            await botClient.Object.SendAsync("any", "any");
            // Assert

            Assert.True(call == 1);
        }

    }
}
