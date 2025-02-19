namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class GetAbnormalAccountUsernameLstCriteria : CriteriaBase
{
    public string MessageIDLst { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_SpecialDelete_AbnormalBets_GetUsernameList";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MessageIDLst);
    }
}