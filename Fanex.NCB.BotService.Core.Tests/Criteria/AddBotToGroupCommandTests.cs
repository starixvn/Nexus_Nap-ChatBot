namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class AddBotToGroupCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new AddBotToGroupCommand();
            var expected = "NCB_Conversation_AddBotToGroup";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ConversationIdIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new AddBotToGroupCommand
            {
                ConversationId = string.Empty,
                BotConversationId = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_BotConversationIdIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new AddBotToGroupCommand
            {
                ConversationId = "aa",
                BotConversationId = string.Empty
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
            var command = new AddBotToGroupCommand
            {
                ConversationId = "aa",
                BotConversationId = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
