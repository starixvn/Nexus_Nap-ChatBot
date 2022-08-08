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
        private const string ListSiteDbCacheKey = "ListSiteDbCachingKey";
        private readonly int CacheExpiration;

        public SiteDbService(IBaseService baseService,
            ICacheService cacheService,
            IDynamicConnectionService dynamicConnectionService,
            IConfiguration configuration)
        {
            this.baseService = baseService;
            this.cacheService = cacheService;
            this.dynamicConnectionService = dynamicConnectionService;
            CacheExpiration = configuration.GetValue<int>("SiteDbCachingExpiration");
        }

        public async Task<IEnumerable<SiteDBConfig>> GetListSiteDbAsync()
        {
            var siteDbList = await cacheService.GetAsync<IEnumerable<SiteDBConfig>>(ListSiteDbCacheKey);

            if (siteDbList is null)
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

        public async Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)> GetTicketUsernamesAynnc(
            IEnumerable<AbnormalTransId> transIds, IEnumerable<AbnormalUsernames> usernames, IEnumerable<SiteDBConfig> siteDbList)
        {
            var (ticketLists, accountLists) = new ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>>(
                new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>());

            var abnormalBetsInfo = XmlHelper<AbnormalTransId>.Serialize(transIds.ToList());
            var abnormalAccounts = XmlHelper<AbnormalUsernames>.Serialize(usernames.ToList());

            foreach (var siteDb in siteDbList)
            {
                var (tickets, accounts) = await GetTicketUsernameInfoFromSiteDbAsync(abnormalBetsInfo, abnormalAccounts, siteDb);
                ticketLists = ticketLists.Concat(tickets);
                accountLists = accountLists.Concat(accounts);
            }

            return DistinctTicketAndCombineUsernames(ticketLists, accountLists);
        }

        public async Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)> GetTicketUsernamesParalellAynnc(
            IEnumerable<AbnormalTransId> transIds, IEnumerable<AbnormalUsernames> usernames, IEnumerable<SiteDBConfig> siteDbList)
        {
            var abnormalBetsInfo = XmlHelper<AbnormalTransId>.Serialize(transIds.ToList());
            var abnormalAccounts = XmlHelper<AbnormalUsernames>.Serialize(usernames.ToList());

            var tasks = new List<Task<ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>>>>();

            foreach (var siteDb in siteDbList)
            {
                tasks.Add(
                    GetTicketUsernameInfoFromSiteDbAsync(abnormalBetsInfo, abnormalAccounts, siteDb)
                    );
            }

            List<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)> results = (await Task.WhenAll(tasks)).ToList();

            var (ticketLists, accountLists) = new ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>>(
                new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>());

            foreach (var result in results)
            {
                ticketLists = ticketLists.Concat(result.Item1);
                accountLists = accountLists.Concat(result.Item2);
            }

            return DistinctTicketAndCombineUsernames(ticketLists, accountLists);
        }

        private async Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)> GetTicketUsernameInfoFromSiteDbAsync(
            string abnormalBetsInfo, string abnormalAccounts, SiteDBConfig siteDb)
        {
            var criteria = new AbnormalBetGetUsernameTicketInfoCriteria
            {
                ServerIp = siteDb.AGIP,
                AbnormalBetsInfo = abnormalBetsInfo,
                AbnormalAccountsInfo = abnormalAccounts,
                SiteDBGroupId = siteDb.SiteDBGroupID
            };

            var result = await dynamicConnectionService.FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames>(criteria);

            return result;
        }

        private static (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>) DistinctTicketAndCombineUsernames(
            IEnumerable<AbnormalTicketInfo> ticketLists, IEnumerable<AbnormalUsernames> accountLists)
        {
            var outputTickets = ticketLists.GroupBy(x => new
            {
                x.MessageId,
                x.Recommend,
                x.IsParlay,
                x.Bettype,
                x.LiveIndicator
            }).Select(g => g.FirstOrDefault());

            var outputUsernames = accountLists.GroupBy(x => new
            {
                x.MessageId,
                x.VoidingHistoryId
            }).Select(g => new AbnormalUsernames
            {
                MessageId = g.Key.MessageId,
                VoidingHistoryId = g.Key.VoidingHistoryId,
                Usernames = string.Join(",", g.Select(i => i.Usernames).ToArray())
            });

            return (outputTickets, outputUsernames);
        }
    }
}