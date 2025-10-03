namespace Fanex.NCB.NAPResolver.Tests;

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Models.ValueObject;
using Core.Enum;
using Criteria.LocalNap.Security;
using Criteria.LocalNap.Site;
using LocalServices;
using Shared.Services.Interfaces;
using NSubstitute;
using Xunit;

public class LocalSecurityServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private IBaseService baseService;

    [Fact]
    public async Task GetTimeAsync_WhenCalled_ReturnExpectedResult()
    {
        var service = GetService();

        var criteria = new UserSsoGetTimeUserCriteria();

        baseService.GetDataAsync<int>(Arg.Do<UserSsoGetTimeUserCriteria>(x => criteria = x))
            .Returns(new List<int> { 1 });

        var time = await service.GetTimeAsync();

        Assert.Equal(criteria.Time.ToString(CultureInfo.InvariantCulture), time);
    }

    #region LoginAsync

    [Fact]
    public async Task LoginAsync_OnUserIdIsInvalid_ReturnAsExpectedResult()
    {
        // Arrange
        var service = GetService();
        var loginParameter = fixture.Create<LoginParameter>();
        var criteria = new UserCheckInCriteria();
        baseService.GetDataAsync<int>(Arg.Do<UserCheckInCriteria>(x => criteria = x))
            .Returns(new List<int> { 0 }).AndDoes(x => criteria.OnUserId = 0);

        // Act
        var onUserId = await service.LoginAsync(loginParameter);

        // Assert
        Assert.Equal(0, onUserId);

        Assert.Equal(loginParameter.UserId, criteria.UserId);
        Assert.Equal(loginParameter.SiteId, criteria.SiteId);
        Assert.Equal(loginParameter.NetSessionId, criteria.NetSessionId);
        Assert.Equal(loginParameter.Ip, criteria.Ip);
        Assert.Equal(loginParameter.LoginDate, criteria.LoginDate);
    }

    [Fact]
    public async Task LoginAsync_OnUserIdIsValid_ReturnAsExpectedResult()
    {
        // Arrange
        var service = GetService();
        var loginParameter = fixture.Create<LoginParameter>();
        var userCheckInCriteria = new UserCheckInCriteria();

        baseService.GetDataAsync<int>(Arg.Do<UserCheckInCriteria>(c => userCheckInCriteria = c))
            .Returns(new List<int> { 1 }).AndDoes(x => userCheckInCriteria.OnUserId = 1);

        var logUserLoginCriteria = new LogUserLoginCriteria();
        baseService.GetDataAsync<int>(Arg.Do<LogUserLoginCriteria>(x => logUserLoginCriteria = x))
            .Returns(new List<int> { 1 });

        // Act
        var onUserId = await service.LoginAsync(loginParameter);

        // Assert
        Assert.Equal(1, onUserId);

        Assert.Equal(loginParameter.UserId, userCheckInCriteria.UserId);
        Assert.Equal(loginParameter.SiteId, userCheckInCriteria.SiteId);
        Assert.Equal(loginParameter.NetSessionId, userCheckInCriteria.NetSessionId);
        Assert.Equal(loginParameter.Ip, userCheckInCriteria.Ip);
        Assert.Equal(loginParameter.LoginDate, userCheckInCriteria.LoginDate);

        Assert.Equal(loginParameter.UserId, logUserLoginCriteria.UserId);
        Assert.Equal(loginParameter.SiteId, logUserLoginCriteria.SiteId);
        Assert.Equal(loginParameter.LoginDate, logUserLoginCriteria.LogDate);
        Assert.Equal(loginParameter.Domain, logUserLoginCriteria.DomainName);
        Assert.Equal(loginParameter.BrowserName, logUserLoginCriteria.BrowserName);
        Assert.Equal(loginParameter.Platform, logUserLoginCriteria.Platform);
        Assert.Equal(loginParameter.Ip, logUserLoginCriteria.IP);
        Assert.Equal(loginParameter.CountryCode, logUserLoginCriteria.CountryCode);
    }

    #endregion LoginAsync

    [Fact]
    public async Task ReCheckInAsync_SiteIsInactive_ReturnKickOutStatus()
    {
        var service = GetService();
        var onlineUserId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var site = fixture.Create<SiteUrl>();
        site.IsActive = false;

        var getByIdSiteCriteria = new GetByIdSiteCriteria();

        baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c))
            .Returns(site);

        var statusId = await service.ReCheckInAsync(onlineUserId, siteId);

        Assert.Equal((int)RecheckInStatus.KickOut, statusId);
    }

    [Fact]
    public async Task ReCheckInAsync_UserIsKickedOut_ReturnKickOutStatus()
    {
        var service = GetService();
        var onlineUserId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var site = fixture.Create<SiteUrl>();
        site.IsActive = true;

        var getByIdSiteCriteria = new GetByIdSiteCriteria();

        baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c))
            .Returns(site);

        var userReCheckInUserCriteria = new UserReCheckInUserCriteria();
        baseService.GetDataAsync<int>(Arg.Do<UserReCheckInUserCriteria>(c => userReCheckInUserCriteria = c))
            .Returns(new List<int> { 1 }).AndDoes(x => userReCheckInUserCriteria.IsKickedOut = true);

        var statusId = await service.ReCheckInAsync(onlineUserId, siteId);

        Assert.Equal((int)RecheckInStatus.KickOut, statusId);
    }

    [Fact]
    public async Task ReCheckInAsync_UserIsNotKickedOut_ReturnSuccessStatus()
    {
        var service = GetService();
        var onlineUserId = fixture.Create<int>();
        var siteId = fixture.Create<int>();
        var site = fixture.Create<SiteUrl>();
        site.IsActive = true;

        var getByIdSiteCriteria = new GetByIdSiteCriteria();

        baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c))
            .Returns(site);

        var userReCheckInUserCriteria = new UserReCheckInUserCriteria();
        baseService.GetDataAsync<int>(Arg.Do<UserReCheckInUserCriteria>(c => userReCheckInUserCriteria = c))
            .Returns(new List<int> { 1 });

        var statusId = await service.ReCheckInAsync(onlineUserId, siteId);

        Assert.Equal((int)RecheckInStatus.Success, statusId);
    }

    [Fact]
    public async Task LogoutAsync_WhenCalled_LogoutExcuteIsCalled()
    {
        var service = GetService();
        var onlineUserId = fixture.Create<int>();
        var criteria = new UserLogOutCriteria();

        baseService.GetDataAsync<int>(Arg.Do<UserLogOutCriteria>(c => criteria = c)).Returns(new List<int> { 1 });

        await service.LogoutAsync(onlineUserId);

        Assert.Equal(onlineUserId, criteria.OnUserId);
    }

    [Fact]
    public async Task SSOLoginAsync_ValidData_ReturnExpectedResult()
    {
        var service = GetService();
        var loginParameter = fixture.Create<SSOLoginParameter>();
        var expectedOnUserId = 1;

        var userCheckInCriteria = new UserSsoCheckInUserCriteria();

        baseService.GetDataAsync<int>(Arg.Do<UserSsoCheckInUserCriteria>(x => userCheckInCriteria = x))
            .Returns(new List<int> { 1 })
            .AndDoes(x => userCheckInCriteria.OnUserId = expectedOnUserId);

        var logUserLoginCriteria = new LogUserLoginCriteria();
        baseService.GetDataAsync<int>(Arg.Do<LogUserLoginCriteria>(x => logUserLoginCriteria = x))
            .Returns(new List<int> { 1 });

        var onUserId = await service.SSOLoginAsync(loginParameter);

        Assert.Equal(expectedOnUserId, onUserId);
    }

    [Fact]
    public void Constructor_WithValidBaseService_CreatesInstance()
    {
        // Arrange
        var mockBaseService = Substitute.For<IBaseService>();

        // Act
        var service = new LocalSecurityService(mockBaseService);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void GetTime_ThrowsNotImplementedException()
    {
        // Arrange
        var service = GetService();

        // Act & Assert
        Assert.Throws<System.NotImplementedException>(() => service.GetTime());
    }

    [Fact]
    public void Login_ThrowsNotImplementedException()
    {
        // Arrange
        var service = GetService();
        var parameter = fixture.Create<LoginParameter>();

        // Act & Assert
        Assert.Throws<System.NotImplementedException>(() => service.Login(parameter));
    }

    [Fact]
    public async Task GetTimeAsync_SetsCorrectTimeFormat()
    {
        // Arrange
        var service = GetService();
        var criteria = new UserSsoGetTimeUserCriteria();

        baseService.GetDataAsync<int>(Arg.Do<UserSsoGetTimeUserCriteria>(x => criteria = x))
            .Returns(new List<int> { 1 });

        // Act
        var time = await service.GetTimeAsync();

        // Assert
        Assert.NotNull(time);
        Assert.NotEmpty(time);
        // Verify the time format matches the expected culture invariant format
        Assert.True(System.DateTime.TryParse(time, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _));
    }

    #region Helpers

    private LocalSecurityService GetService()
    {
        baseService = Substitute.For<IBaseService>();

        return Substitute.For<LocalSecurityService>(baseService);
    }

    #endregion Helpers
}