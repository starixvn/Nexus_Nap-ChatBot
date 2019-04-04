using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.MappingStatusServiceTest
{
    public class MappingStatusService_GetMappingStatusTest
    {
        [Fact]
        public void GetMappingStatus_WhenCalled_ReturnMappingStatusListCorrectly()
        {
            // Arrange
            var service = new MappingStatusService();
            var expectedResult = new List<MappingStatus> {
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.NotMapped, Name = "Not Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.Mapped, Name = "Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.BotRemoved, Name = "Bot Removed" }
            };

            // Act
            var result = service.GetMappingStatus();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
