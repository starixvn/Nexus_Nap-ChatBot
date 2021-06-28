namespace Fanex.NCB.NAPResolver.Tests
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NCB.Criteria.LocalNap.Group;
    using Fanex.NCB.NAPResolver.LocalServices;
    using NSubstitute;
    using Xunit;

    public class LocalGroupServiceTests
    {
        private readonly IObjectDb objectDb;

        public LocalGroupServiceTests()
        {
            DbSettingProviderManager.StartTestingSession().Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = new LocalGroupService();

            var fixture = new Fixture();
            var groupId = fixture.Create<int>();
            var expectedGroup = new Group
            {
                Id = 1,
                Name = "Test",
                IsActive = true
            };

            var criteria = new GetGroupByIdCriteria();

            objectDb.QueryEntityAsync(
                Arg.Any<Func<Group, User, User, Group>>(),
                    "CreatorId, ModifierId",
                Arg.Do<GetGroupByIdCriteria>(x => criteria = x)).Returns(expectedGroup);

            var result = await service.GetAsync(groupId);

            Assert.Equal(expectedGroup.Id, result.Id);
            Assert.Equal(expectedGroup.Name, result.Name);
            Assert.Equal(expectedGroup.IsActive, result.IsActive);
        }
    }
}