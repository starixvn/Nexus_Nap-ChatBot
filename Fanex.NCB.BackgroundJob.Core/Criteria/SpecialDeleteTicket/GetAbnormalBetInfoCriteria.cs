namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class GetAbnormalBetInfoCriteria : CriteriaBase
{
    public string TicketInfo { get; set; }

    public string UsernameLst { get; set; }

    public string DuplicatedInfo { get; set; }

    public string MessageInfo { get; set; }

    public string MessageDetail { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_SpecialDelete_AbnormalBets_Sel";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(TicketInfo);
    }
}