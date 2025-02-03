namespace Fanex.NCB.Criteria.SettingConfiguration;

using Fanex.NCB.Shared.Models;

public class UpdateJobInfoCommand : BaseCommand
{
    public string JobInfo { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_JobInfo_Upd";
    }

    public override bool IsValid()
    {
        return ModifiedBy > 0;
    }
}