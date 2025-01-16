namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class AbnormalBetGetUsernameTicketInfoCriteria : CriteriaBase
    {
        public string AbnormalBetsInfo { get; set; }
        public string AbnormalAccountsInfo { get; set; }
        public string DuplicatedTickets { get; set; }
        public int SiteDBGroupId { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_AbnormalBets_UsernameTicketInfo_Sel";

        public override bool IsValid() => SiteDBGroupId >= 0;
    }
}