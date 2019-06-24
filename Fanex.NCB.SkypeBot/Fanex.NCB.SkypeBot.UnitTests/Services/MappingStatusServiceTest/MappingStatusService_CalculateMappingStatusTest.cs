using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using FluentAssertions;
using Xunit;
using MappingStatusData = Fanex.NCB.Common.Constant.MappingStatusData;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.MappingStatusServiceTest
{
    public class MappingStatusService_CalculateMappingStatusTest
    {
        [Fact]
        public void CalculateMappingStatus_WhenBotConversationMemberNull_ReturnBotRemoved()
        {
            // Arrange
            var service = new MappingStatusService();
            BotConversationMember botConversationMember = null;
            var isConversationMapped = true;

            // Act
            var result = service.CalculateMappingStatus(isConversationMapped, botConversationMember);

            // Assert
            result.Should().Be((int)MappingStatusData.MappingStatus.BotRemoved);
        }

        [Fact]
        public void CalculateMappingStatus_WhenBotConversationMemberIsNotActive_ReturnBotRemoved()
        {
            // Arrange
            var service = new MappingStatusService();
            var botConversationMember = new BotConversationMember { IsActive = false };
            var isConversationMapped = true;

            // Act
            var result = service.CalculateMappingStatus(isConversationMapped, botConversationMember);

            // Assert
            result.Should().Be((int)MappingStatusData.MappingStatus.BotRemoved);
        }

        [Fact]
        public void CalculateMappingStatus_WhenBotConversationMemberActiveAndConversationMapped_ReturnMapped()
        {
            // Arrange
            var service = new MappingStatusService();
            var botConversationMember = new BotConversationMember { IsActive = true };
            var isConversationMapped = true;

            // Act
            var result = service.CalculateMappingStatus(isConversationMapped, botConversationMember);

            // Assert
            result.Should().Be((int)MappingStatusData.MappingStatus.Mapped);
        }

        [Fact]
        public void CalculateMappingStatus_WhenBotConversationMemberActiveAndConversationNotMapped_ReturnNotMapped()
        {
            // Arrange
            var service = new MappingStatusService();
            var botConversationMember = new BotConversationMember { IsActive = true };
            var isConversationMapped = false;

            // Act
            var result = service.CalculateMappingStatus(isConversationMapped, botConversationMember);

            // Assert
            result.Should().Be((int)MappingStatusData.MappingStatus.NotMapped);
        }
    }
}