namespace Fanex.NCB.AutoJobs.Core.Tests.Services
{
    using AutoFixture;
    using Caching;
    using Criteria.SpecialDeleteTicket;
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Helpers;
    using Microsoft.Extensions.Configuration;
    using Models.Common;
    using Models.SpecialDeleteTicket.AbnormalBets;
    using Newtonsoft.Json;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class SiteDbServiceTests
    {
        private ICacheService cacheService;
        private ILogService logService;
        private IDynamicRepository dynamicRepository;

        private const string ListSiteDbCacheKey = "ListSiteDbCachingKey";

        [Fact]
        public async Task GetListSiteDbAsync_WhenNoCache_WillGetDataFromDb()
        {
            var fixture = new Fixture();
            var siteDbs = fixture.Create<IEnumerable<SiteDBConfig>>();
            var service = GetService();
            dynamicRepository.FetchAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);

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

            await dynamicRepository.DidNotReceive().FetchAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>());
            await cacheService.DidNotReceive().SetAsync(Arg.Is(ListSiteDbCacheKey), siteDbs, Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task GetTicketUsernamesAynnc_WhenCalled_WillCallExpectedService()
        {
            var service = GetService();
            var (transIds, usernames, siteDbs, duclicatedTicked) = FakeInputData();
            var fixture = new Fixture();
            var (tickets, username, duplicatedTickets) = fixture.Create<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)>();

            dynamicRepository.FetchAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);
            dynamicRepository.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
                Arg.Any<AbnormalBetGetUsernameTicketInfoCriteria>(), Arg.Any<string>()).Returns((tickets, username, duplicatedTickets));

            var result = await service.GetTicketUsernamesAynnc(transIds, usernames, duclicatedTicked);

            var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
            var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());

            await dynamicRepository.Received(siteDbs.Count()).FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
                Arg.Is<AbnormalBetGetUsernameTicketInfoCriteria>(i =>
                    i.AbnormalBetsInfo == abnormalBetsInfo &&
                    i.AbnormalAccountsInfo == abnormalAccounts
                ), Arg.Any<string>());
        }

        [Fact]
        public async Task GetTicketUsernamesParalellAynnc_WhenCall_ReturnExpectedDistinctAndGroupUsernamesResult()
        {
            var service = GetService();
            var (transIds, usernames, siteDbs, duclicatedTicked) = FakeInputData();
            siteDbs = new List<SiteDBConfig>
            {
                new() { AGIP = "1.1.1.1"},
                new() { AGIP = "1.2.3.4"}
            };
            dynamicRepository.FetchAsync<SiteDBConfig>(Arg.Any<GetSiteDbListCriteria>()).Returns(siteDbs);

            var (expectedTickets, expectedUsernames, expectedDuplicate) = GetExpectedResult();
            PrepareDataForTest();

            var (tickResult, usernameResult, duplicatedTicketInfo) = await service.GetTicketUsernamesParalellAynnc(transIds, usernames, duclicatedTicked);
            Assert.Equal(JsonConvert.SerializeObject(expectedTickets), JsonConvert.SerializeObject(tickResult));
            Assert.Equal(JsonConvert.SerializeObject(expectedUsernames), JsonConvert.SerializeObject(usernameResult));
            Assert.Equal(JsonConvert.SerializeObject(expectedDuplicate), JsonConvert.SerializeObject(duplicatedTicketInfo));
        }

        private (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>) GetExpectedResult()
        {
            var ticket = new List<AbnormalTicketInfo>
            {
                new() { Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 10},
                new() {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100}
            };
            var username = new List<AbnormalUsernames>
            {
                new() {VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 10},
                new() { VoidingHistoryId = 10, Username = "DEF", VoidingHistoryCollectionId = 10},
                new() { VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 12},
                new() { VoidingHistoryId = 10, Username = "DJI", VoidingHistoryCollectionId = 12 }
            };

            var duplicate = new List<DuplicatedTicketInfo>
            {
                new() {VoidingHistoryCollectionId = 20, VoidedTransId = 1, CustId = 1,IsParlay = false},
                new() {VoidingHistoryCollectionId = 20, VoidedTransId = 1, CustId = 3,IsParlay = false},
            };

            return (ticket, username, duplicate);
        }

        private void PrepareDataForTest()
        {
            var ticket1 = new List<AbnormalTicketInfo>
            {
                new() {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 10},
                new() {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100}
            };
            var ticket2 = new List<AbnormalTicketInfo>
            {
                new() {Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1,VoidingHistoryCollectionId = 10},
                new() { Bettype = 1, IsParlay = true, LiveIndicator = true, Recommend = 1, VoidingHistoryCollectionId = 100 }
            };

            var username1 = new List<AbnormalUsernames>
            {
                new() {VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 10},
                new() {VoidingHistoryId = 10, Username = "DEF", VoidingHistoryCollectionId = 10},
                new() {VoidingHistoryId = 10, Username = "ABC", VoidingHistoryCollectionId = 12},
            };

            var username2 = new List<AbnormalUsernames>
            {
                new() {VoidingHistoryId = 10, Username = "DJI", VoidingHistoryCollectionId = 12},
            };
            var duplicate1 = new List<DuplicatedTicketInfo>
            {
                new() {CustId =1, VoidedTransId = 1, VoidingHistoryCollectionId = 20}
            };

            var duplicate2 = new List<DuplicatedTicketInfo>
            {
                new() {CustId = 3, VoidedTransId = 1, VoidingHistoryCollectionId = 20}
            };

            dynamicRepository.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
                Arg.Any<AbnormalBetGetUsernameTicketInfoCriteria>(), Arg.Is("1.1.1.1")).Returns((ticket1, username1, duplicate1));

            dynamicRepository.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(
                Arg.Any<AbnormalBetGetUsernameTicketInfoCriteria>(), Arg.Is("1.2.3.4")).Returns((ticket2, username2, duplicate2));
        }

        private (IEnumerable<CollectionTransId>, IEnumerable<CollectionUsername>, IEnumerable<SiteDBConfig>, IEnumerable<CollectionDuplicatedTickets>) FakeInputData()
        {
            var fixture = new Fixture();
            var tranIds = fixture.Create<IEnumerable<CollectionTransId>>();
            var usernames = fixture.Create<IEnumerable<CollectionUsername>>();
            var siteDbs = fixture.Create<IEnumerable<SiteDBConfig>>();
            var duclicatedTicked = fixture.Create<IEnumerable<CollectionDuplicatedTickets>>();
            return (tranIds, usernames, siteDbs, duclicatedTicked);
        }

        private SiteDbService GetService()
        {
            cacheService = Substitute.For<ICacheService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            logService = Substitute.For<ILogService>();

            var inMemorySettings = new Dictionary<string, string> { { "TopLevelKey", "15" } };
            IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
            return new SiteDbService(cacheService, configuration, dynamicRepository, logService);
        }
    }
}