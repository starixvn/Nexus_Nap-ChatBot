namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class AbnormalBetGetUsernameAndTransIdCriteria : CriteriaBase
    {
        public string MessageIdLst { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_AbnormalBets_Info_Sel";

        public override bool IsValid() => !string.IsNullOrEmpty(MessageIdLst);
    }
}