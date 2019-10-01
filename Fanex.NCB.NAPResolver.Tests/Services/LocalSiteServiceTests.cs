namespace Fanex.NCB.NAPResolver.Tests
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NCB.Criteria.LocalNap.Site;
    using Fanex.NCB.NAPResolver.LocalServices;
    using NSubstitute;
    using Xunit;

    public class LocalSiteServiceTests
    {
        private readonly IObjectDb objectDb;

        public LocalSiteServiceTests()
        {
            DbSettingProviderManager.StartTestingSession().Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task IsActivatedAsync_ValidData_ReturnExpectedResult()
        {
            var service = new LocalSiteService();
            var fixture = new Fixture();

            var siteId = fixture.Create<int>();
            var site = fixture.Create<Site>();
            var criteria = new GetByIdSiteCriteria();

            objectDb.QueryEntityAsync<Site>(Arg.Do<GetByIdSiteCriteria>(x => criteria = x)).Returns(site);

            var result = await service.IsActivatedAsync(siteId).ConfigureAwait(false);

            Assert.Equal(siteId, criteria.SiteId);
            Assert.Equal(site.IsActive, result);
        }
    }
}