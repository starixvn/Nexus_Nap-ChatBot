namespace Fanex.NCB.Shared.Models.Settlement
{
    using System;

    public class MessageSettlementHeader : MessageSettlement
    {
        public DateTime EventDate { get; set; }

        public string SportName { get; set; }

        public int TotalTickets { get; set; }

        public string CreditSiteList { get; set; }

        public string DepositSiteList { get; set; }
    }
}
