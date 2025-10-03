namespace Fanex.NCB.NAPResolver.Tests;

using System.Threading.Tasks;
using AutoFixture;
using Fanex.NAP.Client.Models.ValueObject;
using Criteria.LocalNap.Site;
using LocalServices;
using Shared.Services.Interfaces;
using NSubstitute;
using Xunit;

public class LocalSiteServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private IBaseService baseService;

    [Fact]
    public async Task IsActivatedAsync_WithActiveSite_ReturnsTrue()
    {
        // Arrange
        var service = GetService();
        var siteId = fixture.Create<int>();
        var site = new SiteUrl { IsActive = true };
        var criteria = new GetByIdSiteCriteria();

        baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(x => criteria = x)).Returns(site);

        // Act
        var result = await service.IsActivatedAsync(siteId);

        // Assert
        Assert.Equal(siteId, criteria.SiteId);
        Assert.True(result);
    }

    [Fact]
    public async Task IsActivatedAsync_WithInactiveSite_ReturnsFalse()
    {
        // Arrange
        var service = GetService();
        var siteId = fixture.Create<int>();
        var site = new SiteUrl { IsActive = false };
        var criteria = new GetByIdSiteCriteria();

        baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(x => criteria = x)).Returns(site);

        // Act
        var result = await service.IsActivatedAsync(siteId);

        // Assert
        Assert.Equal(siteId, criteria.SiteId);
        Assert.False(result);
    }

    [Fact]
    public async Task IsActivatedAsync_WithNullSite_ReturnsFalse()
    {
        // Arrange
        var service = GetService();
        var siteId = fixture.Create<int>();
        var criteria = new GetByIdSiteCriteria();

        baseService.GetDataEntityAsync<SiteUrl>(Arg.Do<GetByIdSiteCriteria>(x => criteria = x)).Returns((SiteUrl)null);

        // Act
        var result = await service.IsActivatedAsync(siteId);

        // Assert
        Assert.Equal(siteId, criteria.SiteId);
        Assert.False(result);
    }

    [Fact]
    public void Constructor_WithValidBaseService_CreatesInstance()
    {
        // Arrange
        var mockBaseService = Substitute.For<IBaseService>();

        // Act
        var service = new LocalSiteService(mockBaseService);

        // Assert
        Assert.NotNull(service);
    }

    #region Helpers

    public LocalSiteService GetService()
    {
        baseService = Substitute.For<IBaseService>();
        return new LocalSiteService(baseService);
    }

    #endregion Helpers
}