namespace Fanex.NCB.NAPResolver.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NCB.Criteria.LocalNap.User;
    using Fanex.NCB.NAPResolver.LocalServices;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class LocalUserServiceTests
    {
        private readonly IObjectDb objectDb;

        public LocalUserServiceTests()
        {
            DbSettingProviderManager.StartTestingSession().Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetAsync_UserId_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();

            var userId = fixture.Create<int>();
            var user = new User
            {
                Id = userId,
                Name = "Test"
            };
            var criteria = new GetByUserIdCriteria();

            objectDb.QueryEntityAsync(Arg.Any<Func<User, Group, User, User, User>>(),
                  "GroupId,CreatorId,ModifierId",
                  Arg.Do<GetByUserIdCriteria>(x => criteria = x)).Returns(user);

            var result = await service.GetAsync(userId).ConfigureAwait(false);

            Assert.Equal(userId, criteria.UserId);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetAsync_Username_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();

            var username = fixture.Create<string>();
            var user = new User
            {
                Id = 1,
                Name = username
            };
            var criteria = new GetUserByUsernameCriteria();

            objectDb.QueryEntityAsync(Arg.Any<Func<User, Group, User, User, User>>(),
                  "GroupId,CreatorId,ModifierId",
                  Arg.Do<GetUserByUsernameCriteria>(x => criteria = x)).Returns(user);

            var result = await service.GetAsync(username).ConfigureAwait(false);

            Assert.Equal(username, criteria.Username);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetMenuItemAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var userId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var environment = fixture.Create<string>();
            var isActive = fixture.Create<bool?>();

            var menuItems = fixture.Create<IEnumerable<MenuItem>>();

            var criteria = new GetMenuItemCriteria();

            objectDb.QueryAsync(Arg.Any<Func<MenuItem, MenuItem, Function, Site, User, User, MenuItem>>(),
                  "ParentId, FunctionId, SiteId, CreatorId, ModifierId",
                  Arg.Do<GetMenuItemCriteria>(x => criteria = x)).Returns(menuItems);

            var result = await service.GetMenuItemAsync(userId, siteId, environment, isActive).ConfigureAwait(false);

            Assert.Equal(userId, criteria.UserId);
            Assert.Equal(siteId, criteria.SiteId);
            Assert.Equal(environment, criteria.Environment);
            Assert.Equal(menuItems, result);
        }

        [Fact]
        public void GetPermission_ValidData_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();

            var userId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var functionKey = fixture.Create<string>();

            var permission = fixture.Create<int>();

            var criteria = new GetUserFunctionPolicyCriteria();

            objectDb.QueryEntity<int>(Arg.Do<GetUserFunctionPolicyCriteria>(x => criteria = x)).Returns(permission);

            var result = service.GetPermission(userId, siteId, functionKey);

            Assert.Equal(userId, criteria.UserId);
            Assert.Equal(siteId, criteria.SiteId);
            Assert.Equal(functionKey, criteria.FunctionKey);
            Assert.Equal(permission, result);
        }

        [Fact]
        public async Task GetPermissionAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();

            var userId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var functionKey = fixture.Create<string>();

            var permission = fixture.Create<int>();

            var criteria = new GetUserFunctionPolicyCriteria();

            objectDb.QueryEntityAsync<int>(Arg.Do<GetUserFunctionPolicyCriteria>(x => criteria = x)).Returns(permission);

            var result = await service.GetPermissionAsync(userId, siteId, functionKey).ConfigureAwait(false);

            Assert.Equal(userId, criteria.UserId);
            Assert.Equal(siteId, criteria.SiteId);
            Assert.Equal(functionKey, criteria.FunctionKey);
            Assert.Equal(permission, result);
        }

        [Fact]
        public async Task GetSiteLevelAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var userId = fixture.Create<int>();
            var environment = fixture.Create<string>();

            var siteLevels = fixture.Create<IEnumerable<SiteLevel>>();

            var criteria = new GetSiteLevelCriteria();

            objectDb.QueryAsync(Arg.Any<Func<SiteLevel, Site, User, User, SiteLevel>>(),
                "SiteId, CreatorId, ModifierId",
                Arg.Do<GetSiteLevelCriteria>(x => criteria = x)).Returns(siteLevels);

            var result = await service.GetSiteLevelAsync(userId, environment).ConfigureAwait(false);

            Assert.Equal(userId, criteria.UserId);
            Assert.Equal(environment, criteria.Environment);
            Assert.Equal(siteLevels, result);
        }

        [Fact]
        public void IsHavingPermission_ValidData_ReturnExpectedResult()
        {
            var service = new LocalUserService();
            var fixture = new Fixture();

            var userId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var functionKey = fixture.Create<string>();
            var policy = fixture.Create<Policy>();

            var permission = fixture.Create<int>();
            var criteria = new GetUserFunctionPolicyCriteria();

            objectDb.QueryEntity<int>(Arg.Do<GetUserFunctionPolicyCriteria>(x => criteria = x)).Returns(permission);

            var result = service.IsHavingPermission(userId, siteId, functionKey, policy);

            Assert.Equal(userId, criteria.UserId);
            Assert.Equal(siteId, criteria.SiteId);
            Assert.Equal(functionKey, criteria.FunctionKey);

            if (permission >= (int)policy)
            {
                Assert.True(result);
            }
            else
            {
                Assert.False(result);
            }
        }
    }
}