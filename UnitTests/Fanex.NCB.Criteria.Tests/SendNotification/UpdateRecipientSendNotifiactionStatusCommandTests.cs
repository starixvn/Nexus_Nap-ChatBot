namespace Fanex.NCB.Criteria.Tests.SendNotification
{
    using Fanex.NCB.Criteria.SendNotification;
    using Xunit;

    public class UpdateRecipientSendNotifiactionStatusCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateRecipientSendNotifiactionStatusCommand();
            var expected = "NCB_SendNotification_RecipientSentStatus_Upd";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string sentStatus)
        {
            // Arrange
            var criteria = new UpdateRecipientSendNotifiactionStatusCommand
            {
                SentStatus = sentStatus,
                ErrCode = 1,
                ErrMsg = ""
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
            var criteria = new UpdateRecipientSendNotifiactionStatusCommand
            {
                SentStatus = "A"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}