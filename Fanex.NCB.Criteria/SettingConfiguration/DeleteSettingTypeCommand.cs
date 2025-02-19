namespace Fanex.NCB.Criteria.SettingConfiguration;

using Fanex.NCB.Shared.Models;

public class DeleteSettingTypeCommand : BaseCommand
{
    public int Type { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_SettingType_Del";
    }

    public override bool IsValid()
    {
        return ModifiedBy > 0;
    }
}