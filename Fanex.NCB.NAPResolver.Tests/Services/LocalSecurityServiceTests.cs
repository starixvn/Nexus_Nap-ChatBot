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
        public async Task GetTimeAsync_WhenCalled_ReturnAsExpectedResult()
        {
            var service = new LocalSecurityService();

            var criteria = new UserSsoGetTimeUserCriteria();

            objectDb.ExecuteNonQueryAsync(Arg.Do<UserSsoGetTimeUserCriteria>(x => criteria = x)).Returns(1);

            var time = await service.GetTimeAsync();

            Assert.Equal(criteria.Time.ToString(CultureInfo.InvariantCulture), time);
        }

        [Fact]
        public async Task LoginAsync_ValidData_ReturnAsExpectedResult()
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
        public async Task ReCheckInAsync_SiteIsInActive_ReturnKickOutStatus()
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
    }
}