using Fanex.NCB.AutoJobs.Core.FanexDataExtensions;

namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket
{
    public class AbnormalBetGetUsernameTicketInfoCriteria : CriteriaBaseExtend
    {
        public string AbnormalBetsInfo { get; set; }
        public string AbnormalAccountsInfo { get; set; }
        public string DuplicatedTickets { get; set; }
        public int SiteDBGroupId { get; set; }
        public override string ServerIp { get; set; }
        public override string DbName { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_AbnormalBets_UsernameTicketInfo_Sel";

        public override bool IsValid() => SiteDBGroupId >= 0;
    }
}