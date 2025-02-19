namespace Fanex.NCB.Criteria.SettingConfiguration;

using Fanex.NCB.Shared.Models;

public class CreateSettingTypeCommand : BaseCommand
{
    public int Type { get; set; }

    public string Name { get; set; }

    public int CreatorId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_SettingType_Ins";
    }

    public override bool IsValid()
    {
        return CreatorId > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}