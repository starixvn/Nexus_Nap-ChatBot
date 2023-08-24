namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.NCB.Shared.FanexDataExtensions;

    public class AbnormalBetGetUsernameTicketInfoCriteria : CriteriaBaseExtend
    {
        public override string ServerIp { get; set; }
        public override string DbName { get; set; }
        public string AbnormalBetsInfo { get; set; }
        public string AbnormalAccountsInfo { get; set; }
        public string DuplicatedTickets { get; set; }
        public int SiteDBGroupId { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_AbnormalBets_UsernameTicketInfo_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ServerIp)
                                        && SiteDBGroupId >= 0;
    }
}