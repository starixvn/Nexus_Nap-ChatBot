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

        [Fact]
        public async Task LoginAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalSecurityService();
            var fixture = new Fixture();

            var loginParameter = fixture.Create<LoginParameter>();

            var userCheckInCriteria = new UserCheckInCriteria();

            objectDb.When(x => x.ExecuteNonQueryAsync(Arg.Do<UserCheckInCriteria>(c => userCheckInCriteria = c))).Do(x => userCheckInCriteria.OnUserId++);

            var logUserLoginCriteria = new LogUserLoginCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<LogUserLoginCriteria>(x => logUserLoginCriteria = x)).Returns(1);

            var onUserId = await service.LoginAsync(loginParameter).ConfigureAwait(false);

            Assert.Equal(userCheckInCriteria.OnUserId, onUserId);
        }

        [Fact]
        public async Task ReCheckInAsync_SiteIsInactive_ReturnKickOutStatus()
        {
            var service = new LocalSecurityService();
            var fixture = new Fixture();

            var onlineUserId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            site.IsActive = false;

            var getByIdSiteCriteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c)).Returns(site);

            var statusId = await service.ReCheckInAsync(onlineUserId, siteId).ConfigureAwait(false);

            Assert.Equal((int)RecheckInStatus.KickOut, statusId);
        }

        [Fact]
        public async Task ReCheckInAsync_UserIsKickedOut_ReturnKickOutStatus()
        {
            var service = new LocalSecurityService();
            var fixture = new Fixture();

            var onlineUserId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            site.IsActive = true;

            var getByIdSiteCriteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c)).Returns(site);

            var userReCheckInUserCriteria = new UserReCheckInUserCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<UserReCheckInUserCriteria>(c => userReCheckInUserCriteria = c))
                .Returns(1).AndDoes(x => userReCheckInUserCriteria.IsKickedOut = true);

            var statusId = await service.ReCheckInAsync(onlineUserId, siteId).ConfigureAwait(false);

            Assert.Equal((int)RecheckInStatus.KickOut, statusId);
        }

        [Fact]
        public async Task ReCheckInAsync_UserIsNotKickedOut_ReturnSuccessStatus()
        {
            var service = new LocalSecurityService();
            var fixture = new Fixture();

            var onlineUserId = fixture.Create<int>();
            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            site.IsActive = true;

            var getByIdSiteCriteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(c => getByIdSiteCriteria = c)).Returns(site);

            var userReCheckInUserCriteria = new UserReCheckInUserCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<UserReCheckInUserCriteria>(c => userReCheckInUserCriteria = c))
                .Returns(1);

            var statusId = await service.ReCheckInAsync(onlineUserId, siteId).ConfigureAwait(false);

            Assert.Equal((int)RecheckInStatus.Success, statusId);
        }

        [Fact]
        public async Task LogoutAsync_WhenCalled_LogoutExcuteIsCalled()
        {
            var service = new LocalSecurityService();

            var fixture = new Fixture();
            var onlineUserId = fixture.Create<int>();

            var criteria = new UserLogOutCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserLogOutCriteria>(c => criteria = c)).Returns(1);

            await service.LogoutAsync(onlineUserId).ConfigureAwait(false);

            await objectDb.Received().ExecuteNonQueryAsync(Arg.Any<UserLogOutCriteria>());
            Assert.Equal(onlineUserId, criteria.OnUserId);
        }

        [Fact]
        public async Task SSOLoginAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalSecurityService();
            var fixture = new Fixture();

            var loginParameter = fixture.Create<SSOLoginParameter>();
            var expectedOnUserId = fixture.Create<int>();

            var userCheckInCriteria = new UserSsoCheckInUserCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserSsoCheckInUserCriteria>(x => userCheckInCriteria = x)).Returns(1)
                .AndDoes(x => userCheckInCriteria.OnUserId = expectedOnUserId);

            var logUserLoginCriteria = new LogUserLoginCriteria();
            objectDb.ExecuteNonQueryAsync(Arg.Do<LogUserLoginCriteria>(x => logUserLoginCriteria = x)).Returns(1);

            var onUserId = await service.SSOLoginAsync(loginParameter).ConfigureAwait(false);

            Assert.Equal(expectedOnUserId, onUserId);
        }
    }
}