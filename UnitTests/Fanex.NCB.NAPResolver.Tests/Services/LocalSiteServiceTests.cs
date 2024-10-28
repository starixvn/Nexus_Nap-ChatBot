namespace Fanex.NCB.NAPResolver.Tests
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NCB.Criteria.LocalNap.Site;
    using Fanex.NCB.NAPResolver.LocalServices;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class LocalSiteServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task IsActivatedAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();
            var fixture = new Fixture();

            var siteId = fixture.Create<int>();
            var site = fixture.Create<SiteUrl>();
            var criteria = new GetByIdSiteCriteria();

            baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(x => criteria = x)).Returns(site);

            var result = await service.IsActivatedAsync(siteId);

            Assert.Equal(siteId, criteria.SiteId);
            Assert.Equal(site.IsActive, result);
        }

        #region Helpers
        public LocalSiteService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return Substitute.For<LocalSiteService>(baseService);
        }
        #endregion Helpers
    }
}