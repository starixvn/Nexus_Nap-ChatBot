namespace Fanex.NCB.NAPResolver.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Data;
using Criteria.LocalNap.User;
using LocalServices;
using Model;
using Shared.Services.Interfaces;
using NSubstitute;
using Xunit;
using static Fanex.NAP.Core.Business.Constant;

public class LocalUserServiceTests
{
    private IBaseService baseService;

    [Fact]
    public async Task GetAsync_UserId_ReturnExpectedResult()
    {
        var service = GetService();
        var fixture = new Fixture();

        var userId = 1;
        var user = new FlatUser
        {
            UserId = 1,
            Name = "Test",
            GroupId = 1,
            GroupName = "Test",
            Username = "Test",
            Password = "Test",
            CreatedDate = DateTime.UtcNow,
            LastLoginDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            CreatorId = 1,
            CreatorUsername = "Test",
            ModifierId = 1,
            ModifierUsername = "Test",
            FirstName = "Test",
            LastName = "Test",
            Email = "Test"
        };
        var criteria = new GetByUserIdCriteria();

        baseService.GetDataEntityAsync<FlatUser>(Arg.Any<GetByUserIdCriteria>()).Returns(user)
            .AndDoes(x => criteria.UserId = 1);

        var result = await service.GetAsync(userId);

        Assert.Equal(userId, criteria.UserId);
        Assert.Equal(user.UserId, result.Id);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task GetMenuItemAsync_ValidData_ReturnExpectedResult()
    {
        var service = GetService();
        var fixture = new Fixture();

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var userId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var environment = fixture.Create<string>();
        var isActive = fixture.Create<bool?>();

        var menuItems = fixture.Create<IEnumerable<FlatMenuItem>>();
        var menus = menuItems.Select(i => i.ConvertToNestedModel());

        var criteria = new GetMenuItemCriteria();

        baseService.GetDataAsync<FlatMenuItem>(Arg.Any<GetMenuItemCriteria>()).Returns(menuItems);

        var result = await service.GetMenuItemAsync(userId, siteId, environment, isActive);

        Assert.Equal(menus.FirstOrDefault().Id, result.FirstOrDefault().Id);
    }

    [Fact]
    public void GetPermission_ValidData_ReturnExpectedResult()
    {
        var service = GetService();
        var fixture = new Fixture();

        var userId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var functionKey = fixture.Create<string>();

        var permission = fixture.Create<int>();

        var criteria = new GetUserFunctionPolicyCriteria();

        baseService.GetDataEntity<int>(Arg.Do<GetUserFunctionPolicyCriteria>(x => criteria = x)).Returns(permission);

        var result = service.GetPermission(userId, siteId, functionKey);

        Assert.Equal(userId, criteria.UserId);
        Assert.Equal(siteId, criteria.SiteId);
        Assert.Equal(functionKey, criteria.FunctionKey);
        Assert.Equal(permission, result);
    }

    [Fact]
    public async Task GetPermissionAsync_ValidData_ReturnExpectedResult()
    {
        var service = GetService();
        var fixture = new Fixture();

        var userId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var functionKey = fixture.Create<string>();
        var permission = fixture.Create<int>();

        baseService.GetDataEntityAsync<int>(Arg.Any<GetUserFunctionPolicyCriteria>()).Returns(permission);

        var result = await service.GetPermissionAsync(userId, siteId, functionKey);

        Assert.Equal(permission, result);
    }

    [Fact]
    public async Task GetSiteLevelAsync_ValidData_ReturnExpectedResult()
    {
        var service = GetService();
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var userId = fixture.Create<int>();
        var environment = fixture.Create<string>();

        var siteLevels = fixture.Create<IEnumerable<FlatSiteLevel>>();
        var flatSiteLevels = siteLevels.Select(i => i.ConvertToNestedModel());

        var criteria = new GetSiteLevelCriteria();

        baseService.GetDataAsync<FlatSiteLevel>(Arg.Any<GetSiteLevelCriteria>()).Returns(siteLevels);

        var result = await service.GetSiteLevelAsync(userId, environment);

        Assert.Equal(flatSiteLevels.FirstOrDefault().Level, result.FirstOrDefault().Level);
    }

    [Fact]
    public void IsHavingPermission_ValidData_ReturnExpectedResult()
    {
        var service = GetService();
        var fixture = new Fixture();

        var userId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var functionKey = fixture.Create<string>();
        var policy = fixture.Create<Policy>();

        var permission = fixture.Create<int>();
        var criteria = new GetUserFunctionPolicyCriteria();

        baseService.GetDataEntity<int>(Arg.Do<GetUserFunctionPolicyCriteria>(x => criteria = x)).Returns(permission);

        var result = service.IsHavingPermission(userId, siteId, functionKey, policy);

        Assert.Equal(userId, criteria.UserId);
        Assert.Equal(siteId, criteria.SiteId);
        Assert.Equal(functionKey, criteria.FunctionKey);

        if (permission >= (int)policy)
            Assert.True(result);
        else
            Assert.False(result);
    }

    #region Helpers

    public LocalUserService GetService()
    {
        baseService = Substitute.For<IBaseService>();

        return Substitute.For<LocalUserService>(baseService);
    }

    #endregion Helpers
}