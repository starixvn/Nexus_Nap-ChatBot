namespace Fanex.NCB.Criteria.Tests.SiteMessage
{
    using Fanex.NCB.Criteria.SiteMessage;
    using Xunit;

    public class InsertMessageHistoryCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new InsertMessageHistoryCommand();
            var expected = "NAP_Chatbot_MsgChatBot_Ins";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new InsertMessageHistoryCommand
            {
                JobIdentify = 1,
                MessageIDLst = "test",
                Environment = "UAT"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnFalse()
        {
            // Arrange
            var criteria = new InsertMessageHistoryCommand
            {
                JobIdentify = 0,
                MessageIDLst = "",
                Environment = "UAT"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
