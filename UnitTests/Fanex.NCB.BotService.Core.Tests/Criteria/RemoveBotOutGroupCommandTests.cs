namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class RemoveBotOutGroupCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new RemoveBotOutGroupCommand();

            var expected = "NCB_Conversation_RemoveBotOutGroup";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", "", 0)]
        [InlineData("", "", 1)]
        [InlineData("", "1", 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string botConversationId, string conversationId, byte channelId)
        {
            // Arrange
            var command = new RemoveBotOutGroupCommand
            {
                ConversationId = conversationId,
                BotConversationId = botConversationId,
                ChannelId = channelId
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var command = new RemoveBotOutGroupCommand
            {
                ConversationId = "aa",
                BotConversationId = "bb",
                ChannelId = 1
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}