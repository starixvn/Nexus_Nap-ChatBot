namespace Fanex.NCB.AutoJobs.Core.Models.Settlement;

using System;

public class MessageSettlementHeader : MessageSettlement
{
    public DateTime EventDate { get; set; }

    public int TotalTickets { get; set; }

    public string CreditSiteList { get; set; }

    public string DepositSiteList { get; set; }

    public string SportNameEN { get; set; }

    public bool IsTest { get; set; }

    public string SiteNameGroup { get; set; }
}