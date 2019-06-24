using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.MappingStatusServiceTest
{
    public class MappingStatusService_GetMappingStatusNameTest
    {
        [Theory]
        [MemberData(nameof(MappingStatusData))]
        public void GetMappingStatusName_MappingStatusIDIsNotMapped_ReturnNotMappedString(int mappingStatusId, string mappingStatusName)
        {
            // Arrange
            var service = new MappingStatusService();

            // Act
            var result = service.GetMappingStatusName(mappingStatusId);

            // Assert
            result.Should().Be(mappingStatusName);
        }

        public static IEnumerable<object[]> MappingStatusData
        {
            get
            {
                return new[]
                {
                    new object[] { (int)Common.Constant.MappingStatusData.MappingStatus.NotMapped, "Not Mapped" },
                    new object[] { (int)Common.Constant.MappingStatusData.MappingStatus.Mapped, "Mapped" },
                    new object[] { (int)Common.Constant.MappingStatusData.MappingStatus.BotRemoved, "Bot Removed" }
                };
            }
        }
    }
}