namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class ImportMessageFromSpecialDeleteCriteria : CriteriaBase
{
    public string MessageHeader { get; set; }

    public string MessageDetail { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SpecialDeleteMsg_Import";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MessageHeader) &&
               !string.IsNullOrWhiteSpace(MessageDetail);
    }
}