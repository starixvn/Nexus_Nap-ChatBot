namespace Fanex.NCB.NAPResolver.Tests
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Criteria.LocalNap.Group;
    using LocalServices;
    using Model;
    using Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class LocalGroupServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var groupId = fixture.Create<int>();
            var expectedGroup = new FlatGroup
            {
                GroupId = 1,
                GroupName = "Test",
                IsActive = true,
                Description = "Test",
                CreatedDate = DateTime.UtcNow,
                CreatorId = 1,
                CreatorUsername = "Test",
                ModifierId = 1,
                ModifierUsername = "Test"
            };

            var criteria = new GetGroupByIdCriteria();

            baseService.GetDataEntityAsync<FlatGroup>(Arg.Any<GetGroupByIdCriteria>()).Returns(expectedGroup);

            var result = await service.GetAsync(groupId);

            Assert.Equal(expectedGroup.GroupId, result.Id);
            Assert.Equal(expectedGroup.GroupName, result.Name);
            Assert.Equal(expectedGroup.IsActive, result.IsActive);
        }

        #region Helpers
        private LocalGroupService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return Substitute.For<LocalGroupService>(baseService);
        }
        #endregion Helpers
    }
}