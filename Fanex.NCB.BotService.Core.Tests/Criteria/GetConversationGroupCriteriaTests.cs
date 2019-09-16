namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class GetConversationGroupCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetConversationGroupCriteria();
            var expected = "NCB_Conversation_Sel";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ConversationIdIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new GetConversationGroupCriteria
            {
                ConversationId = string.Empty,
                BotName = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_BotNameIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new GetConversationGroupCriteria
            {
                ConversationId = "aa",
                BotName = string.Empty
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
            var command = new GetConversationGroupCriteria
            {
                ConversationId = "aa",
                BotName = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
