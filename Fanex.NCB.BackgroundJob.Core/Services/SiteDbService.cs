namespace Fanex.NCB.AutoJobs.Core.Services;

using Caching;
using Criteria.SpecialDeleteTicket;
using Fanex.Data.Repository;
using Helpers;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Models.Common;
using Models.SpecialDeleteTicket.AbnormalBets;

public class SiteDbService : ISiteDbService
{
    private readonly ICacheService cacheService;
    private readonly IDynamicRepository dynamicRepository;
    private readonly ILogService logService;
    private const string ListSiteDbCacheKey = "ListSiteDbCachingKey";
    private readonly int cacheExpiration;

    public SiteDbService(ICacheService cacheService,
        IConfiguration configuration,
        IDynamicRepository dynamicRepository,
        ILogService logService)
    {
        this.cacheService = cacheService;
        this.dynamicRepository = dynamicRepository;
        cacheExpiration = configuration.GetValue<int>("SiteDbCachingExpiration");
        this.logService = logService;
    }

    public async Task<IEnumerable<SiteDBConfig>> GetListSiteDbAsync()
    {
        var siteDbList = await cacheService.GetAsync<IEnumerable<SiteDBConfig>>(ListSiteDbCacheKey);

        if (siteDbList is null || !siteDbList.Any())
        {
            siteDbList = await dynamicRepository.FetchAsync<SiteDBConfig>(new GetSiteDbListCriteria());

            await cacheService.SetAsync(ListSiteDbCacheKey, siteDbList,
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromMinutes(cacheExpiration)
                });
        }

        return siteDbList;
    }

    public async
        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)>
        GetTicketUsernamesAynnc(
            IEnumerable<CollectionTransId> transIds, IEnumerable<CollectionUsername> usernames,
            IEnumerable<CollectionDuplicatedTickets> duplicatedTickets)
    {
        var siteDbList = await GetListSiteDbAsync();
        var (ticketLists, accountLists, dupTicketLists) =
            new ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>,
                IEnumerable<DuplicatedTicketInfo>>(
                new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>(), new List<DuplicatedTicketInfo>());

        var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
        var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());
        var dupTicketsInfo = XmlHelper<CollectionDuplicatedTickets>.Serialize(duplicatedTickets.ToList());

        foreach (var siteDb in siteDbList)
        {
            var (tickets, accounts, dupTickets) =
                await GetTicketUsernameInfoFromSiteDbAsync(abnormalBetsInfo, abnormalAccounts, dupTicketsInfo, siteDb);
            ticketLists = ticketLists.Concat(tickets);
            accountLists = accountLists.Concat(accounts);
            dupTicketLists = dupTicketLists.Concat(dupTickets);
        }

        return DistinctTicketAndCombineUsernames(ticketLists, accountLists, dupTicketLists);
    }

    public async
        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)>
        GetTicketUsernamesParalellAynnc(
            IEnumerable<CollectionTransId> transIds, IEnumerable<CollectionUsername> usernames,
            IEnumerable<CollectionDuplicatedTickets> duplicatedTickets)
    {
        var siteDbList = await GetListSiteDbAsync();

        var abnormalBetsInfo = XmlHelper<CollectionTransId>.Serialize(transIds.ToList());
        var abnormalAccounts = XmlHelper<CollectionUsername>.Serialize(usernames.ToList());
        var dupTickets = XmlHelper<CollectionDuplicatedTickets>.Serialize(duplicatedTickets.ToList());

        var tasks =
            new List<Task<ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>,
                IEnumerable<DuplicatedTicketInfo>>>>();

        foreach (var siteDb in siteDbList)
            tasks.Add(
                GetTicketUsernameInfoFromSiteDbAsync(abnormalBetsInfo, abnormalAccounts, dupTickets, siteDb)
            );

        var results = (await Task.WhenAll(tasks)).ToList();

        var (ticketLists, accountLists, dupTicketLists) =
            new ValueTuple<IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>,
                IEnumerable<DuplicatedTicketInfo>>(
                new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>(), new List<DuplicatedTicketInfo>());

        foreach (var result in results)
        {
            ticketLists = ticketLists.Concat(result.Item1);
            accountLists = accountLists.Concat(result.Item2);
            dupTicketLists = dupTicketLists.Concat(result.Item3);
        }

        return DistinctTicketAndCombineUsernames(ticketLists, accountLists, dupTicketLists);
    }

    private async
        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)>
        GetTicketUsernameInfoFromSiteDbAsync(
            string abnormalBetsInfo, string abnormalAccounts, string duplicatedTickets, SiteDBConfig siteDb)
    {
        (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>) results =
            (new List<AbnormalTicketInfo>(), new List<AbnormalUsernames>(), new List<DuplicatedTicketInfo>());
        try
        {
            var criteria = new AbnormalBetGetUsernameTicketInfoCriteria
            {
                AbnormalBetsInfo = abnormalBetsInfo,
                AbnormalAccountsInfo = abnormalAccounts,
                DuplicatedTickets = duplicatedTickets,
                SiteDBGroupId = siteDb.SiteDBGroupID
            };

            results = await dynamicRepository
                .FetchMultipleAsync<AbnormalTicketInfo, AbnormalUsernames, DuplicatedTicketInfo>(criteria, siteDb.AGIP);
        }
        catch (Exception exception)
        {
            await logService.WriteExceptionSentryErrorAsync(exception);
        }

        return results;
    }

    private static (IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)
        DistinctTicketAndCombineUsernames(
            IEnumerable<AbnormalTicketInfo> ticketLists, IEnumerable<AbnormalUsernames> accountLists,
            IEnumerable<DuplicatedTicketInfo> dupTicketLists)
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