namespace Fanex.NCB.Criteria.Tests.ChatGroupMapping
{
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Xunit;

    public class EditChatGroupMappingCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditChatGroupMappingCommand();
            var expected = "NCB_ChatGroupMapping_Upd";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 1, 0, 0)]
        [InlineData(0, 1, 1, 1)]
        [InlineData(1, 0, 1, 1)]
        [InlineData(1, 1, 0, 1)]
        [InlineData(1, 0, 0, 1)]
        [InlineData(0, 0, 0, 1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int groupId, int recipientTypeId, int creatorID, int chatGroupTypeId)
        {
            // Arrange
            var criteria = new EditChatGroupMappingCommand
            {
                GroupID = groupId,
                RecipientTypeID = recipientTypeId,
                CreatorID = creatorID,
                ChatGroupTypeId = chatGroupTypeId
            };
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new EditChatGroupMappingCommand
            {
                GroupID = 1,
                RecipientTypeID = 1,
                CreatorID = 1,
                ChatGroupTypeId = 1
            };
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}