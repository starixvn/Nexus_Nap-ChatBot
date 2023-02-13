namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Caching;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.FanexDataExtensions.Services;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class SiteDbServiceTests
    {
        private IBaseService baseService;
        private ICacheService cacheService;
        private IDynamicConnectionService dynamicConnectionService;
        private ILogService logService;

        private const string ListSiteDbCacheKey = "ListSiteDbCachingKey";

        [Fact]
        public async Task GetListSiteDbAsync_WhenNoCache_WillGetDataFromDb()
        {
            var fixture = new Fixture();
            var siteDbs = fixture.Create<IEnumerable<SiteDBConfig>>();
            var service = GetService();
            baseService.GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);

            var result = await service.GetListSiteDbAsync();
            Assert.Equal(JsonConvert.SerializeObject(siteDbs), JsonConvert.SerializeObject(result));
            await cacheService.Received(1).SetAsync(Arg.Is(ListSiteDbCacheKey), siteDbs, Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task GetListSiteDbAsync_WhenHaveCache_DoNotGetDataFromDb()
        {
            var fixture = new Fixture();
            var siteDbs = fixture.CreateMany<SiteDBConfig>();
            var service = GetService();
            cacheService.GetAsync<IEnumerable<SiteDBConfig>>(Arg.Is(ListSiteDbCacheKey)).Returns(siteDbs);

            var result = await service.GetListSiteDbAsync();
            Assert.Equal(JsonConvert.SerializeObject(siteDbs), JsonConvert.SerializeObject(result));

            await baseService.DidNotReceive().GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>());
            await cacheService.DidNotReceive().SetAsync(Arg.Is(ListSiteDbCacheKey), siteDbs, Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task GetTicketUsernamesAynnc_WhenCalled_WillCallExpectedService()
        {
            var service = GetService();
            var (transIds, usernames, siteDbs) = FakeInputData();
            var fixture = new Fixture();
            var (tickets, u) = fixture.Create<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)>();

            baseService.GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);
            dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames>(Arg.Any<AbnormalBetGetUsernameTicketInfoCriteria>()).Returns((tickets, u));

            var result = await service.GetTicketUsernamesAynnc(transIds, usernames);

            var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
            var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());

            await dynamicConnectionService.Received(siteDbs.Count()).FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames>(
                Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i =>
                    siteDbs.Any(s => s.AGIP == i.ServerIp) &&
                    i.AbnormalBetsInfo == abnormalBetsInfo &&
                    i.AbnormalAccountsInfo == abnormalAccounts
                )
                );
        }

        [Fact]
        public async Task GetTicketUsernamesParalellAynnc_WhenCall_ReturnExpectedDistinctAndGroupUsernamesResult()
        {
            var service = GetService();
            var (transIds, usernames, siteDbs) = FakeInputData();
            siteDbs = new List<SiteDBConfig>
            {
                new SiteDBConfig { AGIP = "1.1.1.1"},
                new SiteDBConfig { AGIP = "1.2.3.4"}
            };
            baseService.GetDataAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);

            var (expectedTickets, expectedUsernames) = GetExpectedResult();
            PrepareDataForTest();

            var (tickResult, usernameResult) = await service.GetTicketUsernamesParalellAynnc(transIds, usernames);
            Assert.Equal(JsonConvert.SerializeObject(expectedTickets), JsonConvert.SerializeObject(tickResult));
            Assert.Equal(JsonConvert.SerializeObject(expectedUsernames), JsonConvert.SerializeObject(usernameResult));
        }

        private (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>) GetExpectedResult()
        {
            var ticket = new List<AbnormalTicketInfo>
            {
                new AbnormalTicketInfo { Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 10},
                new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100}
            };
            var username = new List<AbnormalUsernames>
            {
                new AbnormalUsernames {VoidingHistoryId = 10, Usernames = "ABC,DEF", VoidingHistoryCollectionId = 10},
                new AbnormalUsernames { VoidingHistoryId = 10, Usernames = "ABC,DJI", VoidingHistoryCollectionId = 12},
            };

            return (ticket, username);
        }

        private void PrepareDataForTest()
        {
            var ticket1 = new List<AbnormalTicketInfo>
            {
                new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 10},
                new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100}
            };
            var ticket2 = new List<AbnormalTicketInfo>
            {
                new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1,VoidingHistoryCollectionId = 10},
                new AbnormalTicketInfo {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1,VoidingHistoryCollectionId = 100}
            };

            var username1 = new List<AbnormalUsernames>
            {
                new AbnormalUsernames {VoidingHistoryId = 10, Usernames = "ABC", VoidingHistoryCollectionId = 10},
                new AbnormalUsernames {VoidingHistoryId = 10, Usernames = "DEF", VoidingHistoryCollectionId = 10},
                new AbnormalUsernames {VoidingHistoryId = 10, Usernames = "ABC", VoidingHistoryCollectionId = 12},
            };

            var username2 = new List<AbnormalUsernames>
            {
                new AbnormalUsernames {VoidingHistoryId = 10, Usernames = "DJI", VoidingHistoryCollectionId = 12},
            };

            dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames>(
                Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i => i.ServerIp == "1.1.1.1")).Returns((ticket1, username1));

            dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames>(
                Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i => i.ServerIp == "1.2.3.4")).Returns((ticket2, username2));
        }

        private (IEnumerable<CollectionTransId>, IEnumerable<CollectionUsername>, IEnumerable<SiteDBConfig>) FakeInputData()
        {
            var fixture = new Fixture();
            var tranIds = fixture.Create<IEnumerable<CollectionTransId>>();
            var usernames = fixture.Create<IEnumerable<CollectionUsername>>();
            var siteDbs = fixture.Create<IEnumerable<SiteDBConfig>>();
            return (tranIds, usernames, siteDbs);
        }

        private (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>) FakeOutputData()
        {
            var fixture = new Fixture();
            var tickets = fixture.Create<IEnumerable<AbnormalTicketInfo>>();
            var accounts = fixture.Create<IEnumerable<AbnormalUsernames>>();
            return (tickets, accounts);
        }

        private SiteDbService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            cacheService = Substitute.For<ICacheService>();
            dynamicConnectionService = Substitute.For<IDynamicConnectionService>();
            logService = Substitute.For<ILogService>();

            var inMemorySettings = new Dictionary<string, string> { { "TopLevelKey", "15" } };
            IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
            return new SiteDbService(baseService, cacheService, dynamicConnectionService, configuration, logService);
        }
    }
}