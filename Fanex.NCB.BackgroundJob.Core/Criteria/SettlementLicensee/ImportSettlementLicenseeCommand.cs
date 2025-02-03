namespace Fanex.NCB.AutoJobs.Core.Criteria.SettlementLicensee;

public class ImportSettlementLicenseeCommand : BaseCommand
{
    public string MessageHeader { get; set; }

    public string MessageDetail { get; set; }

    public string MessageTarget { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_Settlement_Licensee_Import";
    }

    public override bool IsValid()
    {
        return true;
    }
}