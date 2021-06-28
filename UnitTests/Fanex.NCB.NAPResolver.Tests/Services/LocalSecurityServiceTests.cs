namespace Fanex.NCB.NAPResolver.Tests
{
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.LocalNap.Security;
    using Fanex.NCB.Criteria.LocalNap.Site;
    using Fanex.NCB.NAPResolver.LocalServices;
    using NSubstitute;
    using Xunit;

    public class LocalSecurityServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private readonly IObjectDb objectDb;

        public LocalSecurityServiceTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetTimeAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = new LocalSecurityService();

            var criteria = new UserSsoGetTimeUserCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserSsoGetTimeUserCriteria>(x => criteria = x)).Returns(1);

            var time = await service.GetTimeAsync();

            Assert.Equal(criteria.Time.ToString(CultureInfo.InvariantCulture), time);
        }

        #region LoginAsync

        [Fact]
        public async Task LoginAsync_OnUserIdIsInvalid_ReturnAsExpectedResult()
        {
            // Arrange
            var service = new LocalSecurityService();
            var loginParameter = fixture.Create<LoginParameter>();
            var criteria = new UserCheckInCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<UserCheckInCriteria>(x => criteria = x)).Returns(1);

            // Act
            var onUserId = await service.LoginAsync(loginParameter);

            // Assert
            Assert.Equal(0, onUserId);

            Assert.Equal(loginParameter.UserId, criteria.UserId);
            Assert.Equal(loginParameter.SiteId, criteria.SiteId);
            Assert.Equal(loginParameter.NetSessionId, criteria.NetSessionId);
            Assert.Equal(loginParameter.Ip, criteria.Ip);
            Assert.Equal(loginParameter.LoginDate, criteria.LoginDate);

            await objectDb.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<LogUserLoginCriteria>());
        }

        [Fact]
        public async Task LoginAsync_OnUserIdIsValid_ReturnAsExpectedResult()
        {
            // Arrange
            var service = new LocalSecurityService();
            var loginParameter = fixture.Create<LoginParameter>();
            var userCheckInCriteria = new UserCheckInCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserCheckInCriteria>(c => userCheckInCriteria = c))
                .Returns(1).AndDoes(x => userCheckInCriteria.OnUserId = 1);

            var logUserLoginCriteria = new LogUserLoginCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<LogUserLoginCriteria>(x => logUserLoginCriteria = x)).Returns(1);

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
            var service = new LocalSecurityService();
            var onlineUserId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            site.IsActive = false;

            var getByIdSiteCriteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c)).Returns(site);

            var statusId = await service.ReCheckInAsync(onlineUserId, siteId);

            Assert.Equal((int)RecheckInStatus.KickOut, statusId);
        }

        [Fact]
        public async Task ReCheckInAsync_UserIsKickedOut_ReturnKickOutStatus()
        {
            var service = new LocalSecurityService();
            var onlineUserId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            site.IsActive = true;

            var getByIdSiteCriteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c)).Returns(site);

            var userReCheckInUserCriteria = new UserReCheckInUserCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<UserReCheckInUserCriteria>(c => userReCheckInUserCriteria = c))
                .Returns(1).AndDoes(x => userReCheckInUserCriteria.IsKickedOut = true);

            var statusId = await service.ReCheckInAsync(onlineUserId, siteId);

            Assert.Equal((int)RecheckInStatus.KickOut, statusId);
        }

        [Fact]
        public async Task ReCheckInAsync_UserIsNotKickedOut_ReturnSuccessStatus()
        {
            var service = new LocalSecurityService();
            var onlineUserId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            site.IsActive = true;

            var getByIdSiteCriteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c)).Returns(site);

            var userReCheckInUserCriteria = new UserReCheckInUserCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<UserReCheckInUserCriteria>(c => userReCheckInUserCriteria = c))
                .Returns(1);

            var statusId = await service.ReCheckInAsync(onlineUserId, siteId);

            Assert.Equal((int)RecheckInStatus.Success, statusId);
        }

        [Fact]
        public async Task LogoutAsync_WhenCalled_LogoutExcuteIsCalled()
        {
            var service = new LocalSecurityService();
            var onlineUserId = fixture.Create<int>();
            var criteria = new UserLogOutCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserLogOutCriteria>(c => criteria = c)).Returns(1);

            await service.LogoutAsync(onlineUserId);

            await objectDb.Received().ExecuteNonQueryAsync(Arg.Any<UserLogOutCriteria>());
            Assert.Equal(onlineUserId, criteria.OnUserId);
        }

        [Fact]
        public async Task SSOLoginAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalSecurityService();
            var loginParameter = fixture.Create<SSOLoginParameter>();
            var expectedOnUserId = fixture.Create<int>();

            var userCheckInCriteria = new UserSsoCheckInUserCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserSsoCheckInUserCriteria>(x => userCheckInCriteria = x)).Returns(1)
                .AndDoes(x => userCheckInCriteria.OnUserId = expectedOnUserId);

            var logUserLoginCriteria = new LogUserLoginCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<LogUserLoginCriteria>(x => logUserLoginCriteria = x)).Returns(1);

            var onUserId = await service.SSOLoginAsync(loginParameter);

            Assert.Equal(expectedOnUserId, onUserId);
        }
    }
}