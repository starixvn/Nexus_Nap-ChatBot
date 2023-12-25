namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.FanexDataExtensions.Services;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class SiteDbService : ISiteDbService
    {
        private readonly IBaseService baseService;
        private readonly ICacheService cacheService;
        private readonly IDynamicConnectionService dynamicConnectionService;
        private readonly ILogService logService;
        private const string ListSiteDbCacheKey = "ListSiteDbCachingKey";
        private readonly int CacheExpiration;

        public SiteDbService(IBaseService baseService,
            ICacheService cacheService,
            IDynamicConnectionService dynamicConnectionService,
            IConfiguration configuration,
            ILogService logService)
        {
            this.baseService = baseService;
            this.cacheService = cacheService;
            this.dynamicConnectionService = dynamicConnectionService;
            CacheExpiration = configuration.GetValue<int>("SiteDbCachingExpiration");
            this.logService = logService;
        }

        public async Task<IEnumerable<SiteDBConfig>> GetListSiteDbAsync()
        {
            var siteDbList = await cacheService.GetAsync<IEnumerable<SiteDBConfig>>(ListSiteDbCacheKey);

            if (siteDbList is null || !siteDbList.Any())
            {
                siteDbList = await baseService.GetDataAsync<SiteDBConfig>(new GetSiteDbListCriteria());

                await cacheService.SetAsync(ListSiteDbCacheKey, siteDbList,
                    new CacheItemOptions
                    {
                        RelativeExpiration = TimeSpan.FromMinutes(CacheExpiration)
                    });
            }

            return siteDbList;
        }

        public async Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)> GetTicketUsernamesAynnc(
            IEnumerable<CollectionTransId> transIds, IEnumerable<CollectionUsername> usernames, IEnumerable<CollectionDuplicatedTickets> duplicatedTickets)
        {
            var siteDbList = await GetListSiteDbAsync();
            var (ticketLists, accountLists, dupTicketLists) = new ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>>(
                new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>(), new List<DuplicatedTicketInfo>());

            var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
            var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());
            var dupTicketsInfo = XmlHelper<CollectionDuplicatedTickets>.Serialize(duplicatedTickets.ToList());

            foreach (var siteDb in siteDbList)
            {
                var (tickets, accounts, dupTickets) = await GetTicketUsernameInfoFromSiteDbAsync(abnormalBetsInfo, abnormalAccounts, dupTicketsInfo, siteDb);
                ticketLists = ticketLists.Concat(tickets);
                accountLists = accountLists.Concat(accounts);
                dupTicketLists = dupTicketLists.Concat(dupTickets);
            }

            return DistinctTicketAndCombineUsernames(ticketLists, accountLists, dupTicketLists);
        }

        public async Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)> GetTicketUsernamesParalellAynnc(
            IEnumerable<CollectionTransId> transIds, IEnumerable<CollectionUsername> usernames, IEnumerable<CollectionDuplicatedTickets> duplicatedTickets)
        {
            var siteDbList = await GetListSiteDbAsync();

            var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
            var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());
            var dupTickets = XmlHelper<CollectionDuplicatedTickets>.Serialize(duplicatedTickets.ToList());

            var tasks = new List<Task<ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>>>>();

            foreach (var siteDb in siteDbList)
            {
                tasks.Add(
                    GetTicketUsernameInfoFromSiteDbAsync(abnormalBetsInfo, abnormalAccounts, dupTickets, siteDb)
                    );
            }

            List<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)> results = (await Task.WhenAll(tasks)).ToList();

            var (ticketLists, accountLists, dupTicketLists) = new ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>>(
                new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>(), new List<DuplicatedTicketInfo>());

            foreach (var result in results)
            {
                ticketLists = ticketLists.Concat(result.Item1);
                accountLists = accountLists.Concat(result.Item2);
                dupTicketLists = dupTicketLists.Concat(result.Item3);
            }

            return DistinctTicketAndCombineUsernames(ticketLists, accountLists, dupTicketLists);
        }

        private async Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)> GetTicketUsernameInfoFromSiteDbAsync(
            string abnormalBetsInfo, string abnormalAccounts, string duplicatedTickets, SiteDBConfig siteDb)
        {
            (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>) results =
                (new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>(), new List<DuplicatedTicketInfo>());
            try
            {
                var criteria = new AbnormalBetGetUsernameTicketInfoCriteria
                {
                    ServerIp = siteDb.AGIP,
                    AbnormalBetsInfo = abnormalBetsInfo,
                    AbnormalAccountsInfo = abnormalAccounts,
                    DuplicatedTickets = duplicatedTickets,
                    SiteDBGroupId = siteDb.SiteDBGroupID
                };

                results = await dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(criteria);
            }
            catch (Exception exception)
            {
                await logService.WriteExceptionSentryErrorAsync(exception);
            }

            return results;
        }

        private static (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>) DistinctTicketAndCombineUsernames(
            IEnumerable<AbnormalTicketInfo> ticketLists, IEnumerable<AbnormalUsernames> accountLists, IEnumerable<DuplicatedTicketInfo> dupTicketLists)
        {
            var outputTickets = ticketLists.GroupBy(x => new
            {
                x.VoidingHistoryCollectionId,
                x.Recommend,
                x.IsParlay,
                x.Bettype,
                x.LiveIndicator
            }).Select(g => g.FirstOrDefault());

            var outputDupTickets = dupTicketLists.GroupBy(x => new
            {
                x.VoidingHistoryCollectionId,
                x.VoidingHistoryId,
                x.CustId,
                x.OrgTransId,
                x.VoidedTransId,
                x.Status,
                x.Stake,
                x.IsParlay
            }).Select(g => g.FirstOrDefault());

            return (outputTickets, accountLists, outputDupTickets);
        }
    }
}