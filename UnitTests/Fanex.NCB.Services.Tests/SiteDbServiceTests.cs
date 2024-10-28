namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Caching;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    //public class SiteDbServiceTests
    //{
    //    private IBaseService baseService;
    //    private ICacheService cacheService;
    //    private IDynamicConnectionService dynamicConnectionService;
    //    private ILogService logService;

    //    private const string ListSiteDbCacheKey = "ListSiteDbCachingKey";

    //    [Fact]
    //    public async Task GetListSiteDbAsync_WhenNoCache_WillGetDataFromDb()
    //    {
    //        var fixture = new Fixture();
    //        var siteDbs = fixture.Create<IEnumerable<SiteDBConfig>>();
    //        var service = GetService();
    //        baseService.GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);

    //        var result = await service.GetListSiteDbAsync();
    //        Assert.Equal(JsonConvert.SerializeObject(siteDbs), JsonConvert.SerializeObject(result));
    //        await cacheService.Received(1).SetAsync(Arg.Is(ListSiteDbCacheKey), siteDbs, Arg.Any<CacheItemOptions>());
    //    }

    //    [Fact]
    //    public async Task GetListSiteDbAsync_WhenHaveCache_DoNotGetDataFromDb()
    //    {
    //        var fixture = new Fixture();
    //        var siteDbs = fixture.CreateMany<SiteDBConfig>();
    //        var service = GetService();
    //        cacheService.GetAsync<IEnumerable<SiteDBConfig>>(Arg.Is(ListSiteDbCacheKey)).Returns(siteDbs);

    //        var result = await service.GetListSiteDbAsync();
    //        Assert.Equal(JsonConvert.SerializeObject(siteDbs), JsonConvert.SerializeObject(result));

    //        await baseService.DidNotReceive().GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>());
    //        await cacheService.DidNotReceive().SetAsync(Arg.Is(ListSiteDbCacheKey), siteDbs, Arg.Any<CacheItemOptions>());
    //    }

    //    [Fact]
    //    public async Task GetTicketUsernamesAynnc_WhenCalled_WillCallExpectedService()
    //    {
    //        var service = GetService();
    //        var (transIds, usernames, siteDbs, duclicatedTicked) = FakeInputData();
    //        var fixture = new Fixture();
    //        var (tickets, username, duplicatedTickets) = fixture.Create<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)>();

    //        baseService.GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);
    //        dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(Arg.Any<AbnormalBetGetUsernameTicketInfoCriteria>()).Returns((tickets, username, duplicatedTickets));

    //        var result = await service.GetTicketUsernamesAynnc(transIds, usernames, duclicatedTicked);

    //        var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
    //        var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());

    //        await dynamicConnectionService.Received(siteDbs.Count()).FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
    //            Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i =>
    //                siteDbs.Any(s => s.AGIP == i.ServerIp) &&
    //                i.AbnormalBetsInfo == abnormalBetsInfo &&
    //                i.AbnormalAccountsInfo == abnormalAccounts
    //            )
    //            );
    //    }

    //    [Fact]
    //    public async Task GetTicketUsernamesParalellAynnc_WhenCall_ReturnExpectedDistinctAndGroupUsernamesResult()
    //    {
    //        var service = GetService();
    //        var (transIds, usernames, siteDbs, duclicatedTicked) = FakeInputData();
    //        siteDbs = new List<SiteDBConfig>
    //        {
    //            new SiteDBConfig { AGIP = "1.1.1.1"},
    //            new SiteDBConfig { AGIP = "1.2.3.4"}
    //        };
    //        baseService.GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);

    //        var (expectedTickets, expectedUsernames, expectedDuplicate) = GetExpectedResult();
    //        PrepareDataForTest();

    //        var (tickResult, usernameResult, duplicatedTicketInfo) = await service.GetTicketUsernamesParalellAynnc(transIds, usernames, duclicatedTicked);
    //        Assert.Equal(JsonConvert.SerializeObject(expectedTickets), JsonConvert.SerializeObject(tickResult));
    //        Assert.Equal(JsonConvert.SerializeObject(expectedUsernames), JsonConvert.SerializeObject(usernameResult));
    //        Assert.Equal(JsonConvert.SerializeObject(expectedDuplicate), JsonConvert.SerializeObject(duplicatedTicketInfo));
    //    }

    //    private (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>) GetExpectedResult()
    //    {
    //        var ticket = new List<AbnormalTicketInfo>
    //        {
    //            new AbnormalTicketInfo { Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 10},
    //            new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100}
    //        };
    //        var username = new List<AbnormalUsernames>
    //        {
    //            new AbnormalUsernames {VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 10},
    //            new AbnormalUsernames { VoidingHistoryId = 10, Username = "DEF", VoidingHistoryCollectionId = 10},
    //            new AbnormalUsernames { VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 12},
    //            new AbnormalUsernames { VoidingHistoryId = 10, Username = "DJI", VoidingHistoryCollectionId = 12}
    //        };

    //        var duplicate = new List<DuplicatedTicketInfo>
    //        {
    //            new DuplicatedTicketInfo {VoidingHistoryCollectionId = 20, VoidedTransId = 1, CustId = 1,IsParlay = false},
    //            new DuplicatedTicketInfo {VoidingHistoryCollectionId = 20, VoidedTransId = 1, CustId = 3,IsParlay = false},
    //        };

    //        return (ticket, username, duplicate);
    //    }

    //    private void PrepareDataForTest()
    //    {
    //        var ticket1 = new List<AbnormalTicketInfo>
    //        {
    //            new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 10},
    //            new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100}
    //        };
    //        var ticket2 = new List<AbnormalTicketInfo>
    //        {
    //            new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1,VoidingHistoryCollectionId = 10},
    //            new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1,VoidingHistoryCollectionId = 100}
    //        };

    //        var username1 = new List<AbnormalUsernames>
    //        {
    //            new AbnormalUsernames {VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 10},
    //            new AbnormalUsernames {VoidingHistoryId = 10, Username = "DEF", VoidingHistoryCollectionId = 10},
    //            new AbnormalUsernames {VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 12},
    //        };

    //        var username2 = new List<AbnormalUsernames>
    //        {
    //            new AbnormalUsernames {VoidingHistoryId = 10, Username = "DJI", VoidingHistoryCollectionId = 12},
    //        };
    //        var duplicate1 = new List<DuplicatedTicketInfo>
    //        {
    //            new DuplicatedTicketInfo {CustId =1, VoidedTransId = 1, VoidingHistoryCollectionId = 20}
    //        };

    //        var duplicate2 = new List<DuplicatedTicketInfo>
    //        {
    //            new DuplicatedTicketInfo {CustId = 3, VoidedTransId = 1, VoidingHistoryCollectionId = 20}
    //        };

    //        dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
    //            Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i => i.ServerIp == "1.1.1.1")).Returns((ticket1, username1, duplicate1));

    //        dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
    //            Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i => i.ServerIp == "1.2.3.4")).Returns((ticket2, username2, duplicate2));
    //    }

    //    private (IEnumerable<CollectionTransId>, IEnumerable<CollectionUsername>, IEnumerable<SiteDBConfig>, IEnumerable<CollectionDuplicatedTickets>) FakeInputData()
    //    {
    //        var fixture = new Fixture();
    //        var tranIds = fixture.Create<IEnumerable<CollectionTransId>>();
    //        var usernames = fixture.Create<IEnumerable<CollectionUsername>>();
    //        var siteDbs = fixture.Create<IEnumerable<SiteDBConfig>>();
    //        var duclicatedTicked = fixture.Create<IEnumerable<CollectionDuplicatedTickets>>();
    //        return (tranIds, usernames, siteDbs, duclicatedTicked);
    //    }

    //    private SiteDbService GetService()
    //    {
    //        baseService = Substitute.For<IBaseService>();
    //        cacheService = Substitute.For<ICacheService>();
    //        dynamicConnectionService = Substitute.For<IDynamicConnectionService>();
    //        logService = Substitute.For<ILogService>();

    //        var inMemorySettings = new Dictionary<string, string> { { "TopLevelKey", "15" } };
    //        IConfiguration configuration = new ConfigurationBuilder()
    //        .AddInMemoryCollection(inMemorySettings)
    //        .Build();
    //        return new SiteDbService(baseService, cacheService, dynamicConnectionService, configuration, logService);
    //    }
    //}
}