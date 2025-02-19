namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class GetAbnormalBetMessageDetailCriteria : CriteriaBase
{
    public string MessageIdLst { get; set; }

    public int BatchSize { get; set; }

    public int LastMsgTargetId { get; set; }

    public int MaxMsgtargetId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_SpecialDelete_InternalCSD_MessageSel";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MessageIdLst);
    }
}